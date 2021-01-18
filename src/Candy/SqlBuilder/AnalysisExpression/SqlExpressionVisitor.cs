using Candy.Common;
using Candy.DbHelper;
using Candy.Extensions;
using Candy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace Candy.SqlBuilder.AnalysisExpression
{
	internal class SqlExpressionVisitor : ExpressionVisitor
	{
		private SqlExpressionVisitor() { }

		/// <summary>
		/// Visitor静态实例
		/// </summary>
		public static SqlExpressionVisitor Instance => new SqlExpressionVisitor();

		/// <summary>
		/// 输出对象
		/// </summary>
		private SqlExpressionModel _exp;

		/// <summary>
		/// 关联表已拥有别名
		/// </summary>
		private string[] _currentAlias;

		/// <summary>
		/// 输入解析类型
		/// </summary>
		private ExpressionExcutionType _type = ExpressionExcutionType.None;

		/// <summary>
		/// 当前lambda表达式的操作符
		/// </summary>
		private ExpressionType? _currentLambdaNodeType;

		/// <summary>
		/// 是否直接获取转换表达式的值
		/// </summary>
		private bool _isGetConvertException = false;

		/// <summary>
		/// String.Contains
		/// </summary>
		private string _methodStringContainsFormat = null;
		private bool _isAddCastText = false;
		/// <summary>
		/// 获取string数组比较时 倒数第二个空格转化成::text[]
		/// </summary>
		private static readonly Regex _getStringArrayLastSpaceRegex = new Regex(@"\s(?=(<>|\=)\s$)");
		/// <summary>
		/// 表达式操作符转换集
		/// </summary>
		private readonly static Dictionary<ExpressionType, string> _dictOperator = new Dictionary<ExpressionType, string>()
		{
			{ ExpressionType.And," & "},
			{ ExpressionType.AndAlso," AND "},
			{ ExpressionType.Equal," = "},
			{ ExpressionType.GreaterThan," > "},
			{ ExpressionType.GreaterThanOrEqual," >= "},
			{ ExpressionType.LessThan," < "},
			{ ExpressionType.LessThanOrEqual," <= "},
			{ ExpressionType.NotEqual," <> "},
			{ ExpressionType.OrElse," OR "},
			{ ExpressionType.Or," | "},
			{ ExpressionType.Add," + "},
			{ ExpressionType.Subtract," - "},
			{ ExpressionType.Divide," / "},
			{ ExpressionType.Multiply," * "},
			{ ExpressionType.Not," NOT "}

		};

		/// <summary>
		/// 访问单个无别名字段
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public SqlExpressionModel VisitSingleForNoAlias(Expression node)
		{
			Initialize(ExpressionExcutionType.SingleForNoAlias);
			base.Visit(node);
			return _exp;
		}

		/// <summary>
		/// 访问单个有别名字段
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public SqlExpressionModel VisitSingle(Expression node)
		{
			Initialize(ExpressionExcutionType.Single);
			base.Visit(node);
			return _exp;
		}

		/// <summary>
		/// 访问关联查询表达式
		/// </summary>
		/// <param name="node"></param>
		/// <param name="currentAlias"></param>
		/// <returns></returns>
		public SqlExpressionModel VisitUnion(Expression node, IEnumerable<string> currentAlias)
		{
			Initialize(ExpressionExcutionType.Union);
			_currentAlias = currentAlias.ToArray();
			base.Visit(node);
			return _exp;
		}

		/// <summary>
		/// 访问条件表达式
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public SqlExpressionModel VisitCondition(Expression node)
		{
			Initialize(ExpressionExcutionType.Condition);
			base.Visit(node);
			return _exp;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="type"></param>
		private void Initialize(ExpressionExcutionType type)
		{
			_type = type;
			_exp = new SqlExpressionModel();
			_methodStringContainsFormat = null;
			_currentLambdaNodeType = null;
		}

		#region OverrideVisitMethod
		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.Coalesce)
			{
				_exp.SqlText += "COALESCE(";
				base.Visit(node.Left);
				_exp.SqlText += ",";
				base.Visit(node.Right);
				_exp.SqlText += ")";
				return node;
			}
			if (node.NodeType == ExpressionType.ArrayIndex)
			{
				if (node.Left is MemberExpression exp && IsDbMember(exp, out MemberExpression dbMember))
				{
					VisitMember(dbMember);
					_exp.SqlText += string.Concat("[", GetExpressionInvokeResult<int>(node.Right) + 1, "]");
				}
				else
					SetMemberValue(node, node.Type);

				return node;
			}
			if (TransferEnum(node))
				return node;
			_currentLambdaNodeType = node.NodeType;
			VisitLeftAndRight(node.NodeType, node.Left, node.Right);
			return node;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (!ChecktAndSetNullValue(node.Value))
				SetParameter(node.Value);
			return base.VisitConstant(node);
		}

		protected override Expression VisitInvocation(InvocationExpression node)
		{
			SetMemberValue(node, node.Type);
			return node;
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			return base.VisitLambda(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			switch (_type)
			{
				case ExpressionExcutionType.Union:
				case ExpressionExcutionType.Condition:
					switch (node.Member)
					{
						case FieldInfo fieldInfo:
							SetMemberValue(node, fieldInfo.FieldType);
							break;
						case PropertyInfo propertyInfo:
							if (IsDbMember(node, out MemberExpression dbMember))
							{
								_exp.SqlText += _methodStringContainsFormat != null ? string.Format(_methodStringContainsFormat, dbMember.ToDatebaseField()) : dbMember.ToDatebaseField();
								if (_isAddCastText)
								{
									_isAddCastText = false;
									_exp.SqlText += "::text[]";
								}
							}
							else SetMemberValue(node, propertyInfo.PropertyType);
							break;
						default:
							_exp.SqlText += node.ToDatebaseField();
							break;
					}
					break;
				case ExpressionExcutionType.SingleForNoAlias:
					_exp.SqlText += node.Member.Name.ToLower();
					break;
				default:
					{
						if (IsDbMember(node, out MemberExpression dbMember))
							_exp.SqlText += dbMember.ToDatebaseField();
					}
					break;
			}

			return node;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			switch (node.Method.Name)
			{
				case "Contains":
					if (!StringLikeCalling(node, "%{0}%", "'%'||{0}||'%'"))
						MethodContaionsHandler(node);
					break;
				case "StartsWith":
					StringLikeCalling(node, "{0}%", "{0}||'%'");
					break;
				case "EndsWith":
					StringLikeCalling(node, "%{0}", "'%'||{0}");
					break;
				case "ToString" when node.Object.NodeType == ExpressionType.MemberAccess && IsDbMember(node.Object as MemberExpression, out MemberExpression dbMember):
					_exp.SqlText += string.Concat(dbMember.ToDatebaseField(), "::text");
					break;
				default:
					SetExpressionInvokeResultParameter(node);
					break;
			}
			return node;
		}

		protected override Expression VisitConditional(ConditionalExpression node)
		{
			SetExpressionInvokeResultParameter(node);
			return node;
		}

		protected override Expression VisitNew(NewExpression node)
		{
			SetMemberValue(node, node.Type);
			return node;
		}

		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			var nodeType = node.Type.GetElementType();
			if (node.Expressions.Any(a => a.NodeType != ExpressionType.Constant))
				node = Expression.NewArrayInit(nodeType, node.Expressions.Select(a => Expression.Constant(GetExpressionInvokeResultObject(a), a.Type)));
			if (nodeType == typeof(string))
			{
				if (!string.IsNullOrEmpty(_exp.SqlText) && _getStringArrayLastSpaceRegex.IsMatch(_exp.SqlText))
					_exp.SqlText = _getStringArrayLastSpaceRegex.Replace(_exp.SqlText, "::text[]");
				else
					_isAddCastText = true;
			}
			SetMemberValue(node, node.Type);
			return node;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			switch (_type)
			{
				case ExpressionExcutionType.Single:
					_exp.Alias = node.Name;
					break;
				case ExpressionExcutionType.Union when !_currentAlias.Contains(node.Name):
					_exp.Alias = node.Name;
					_exp.UnionType = node.Type;
					break;
			}
			return node;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.NodeType == ExpressionType.ArrayLength)
			{
				_exp.SqlText += "array_length(";
				base.VisitUnary(node);
				_exp.SqlText += ", 1)";
				return node;
			}
			if (node.NodeType == ExpressionType.Not)
				_currentLambdaNodeType = node.NodeType;
			if (node.NodeType == ExpressionType.Convert && _isGetConvertException && !IsDbMember(node.Operand, out MemberExpression _))
			{
				_isGetConvertException = false;
				if (node.Operand is ConstantExpression ce)
					SetMemberValue(ce, ce.Type);
				else
					SetMemberValue(node, node.Type);
				return node;
			}
			return base.VisitUnary(node);
		}

		#region Not Supported Temporarily
		protected override Expression VisitBlock(BlockExpression node)
		{
			throw new NotSupportedException(nameof(VisitBlock));
		}


		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			throw new NotSupportedException(nameof(VisitCatchBlock));
		}



		protected override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			throw new NotSupportedException(nameof(VisitDebugInfo));
		}

		protected override Expression VisitDefault(DefaultExpression node)
		{
			throw new NotSupportedException(nameof(VisitDefault));
		}

		protected override Expression VisitDynamic(DynamicExpression node)
		{
			throw new NotSupportedException(nameof(VisitDynamic));
		}

		protected override ElementInit VisitElementInit(ElementInit node)
		{
			throw new NotSupportedException(nameof(VisitElementInit));
		}

		protected override Expression VisitExtension(Expression node)
		{
			throw new NotSupportedException(nameof(VisitExtension));
		}

		protected override Expression VisitGoto(GotoExpression node)
		{
			throw new NotSupportedException(nameof(VisitGoto));
		}

		protected override Expression VisitIndex(IndexExpression node)
		{
			throw new NotSupportedException(nameof(VisitIndex));
		}

		protected override Expression VisitLabel(LabelExpression node)
		{
			throw new NotSupportedException(nameof(VisitLabel));
		}

		protected override LabelTarget VisitLabelTarget(LabelTarget node)
		{
			throw new NotSupportedException(nameof(VisitLabelTarget));
		}

		protected override Expression VisitListInit(ListInitExpression node)
		{
			throw new NotSupportedException(nameof(VisitListInit));
		}

		protected override Expression VisitLoop(LoopExpression node)
		{
			throw new NotSupportedException(nameof(VisitLoop));
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			throw new NotSupportedException(nameof(VisitMemberAssignment));
		}

		protected override MemberBinding VisitMemberBinding(MemberBinding node)
		{
			throw new NotSupportedException(nameof(VisitMemberBinding));
		}

		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			throw new NotSupportedException(nameof(VisitMemberInit));
		}

		protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			throw new NotSupportedException(nameof(VisitMemberListBinding));
		}

		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			throw new NotSupportedException(nameof(VisitMemberMemberBinding));
		}

		protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			throw new NotSupportedException(nameof(VisitRuntimeVariables));
		}

		protected override Expression VisitSwitch(SwitchExpression node)
		{
			throw new NotSupportedException(nameof(VisitSwitch));
		}

		protected override SwitchCase VisitSwitchCase(SwitchCase node)
		{
			throw new NotSupportedException(nameof(VisitSwitchCase));
		}

		protected override Expression VisitTry(TryExpression node)
		{
			throw new NotSupportedException(nameof(VisitTry));
		}

		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			throw new NotSupportedException(nameof(VisitTypeBinary));
		}
		#endregion

		#endregion

		#region Private Method
		/// <summary>
		/// 判断是否需要添加括号
		/// </summary>
		/// <param name="nodeType"></param>
		/// <returns></returns>
		private bool IsAddBrackets(ExpressionType nodeType) => nodeType == ExpressionType.AndAlso || nodeType == ExpressionType.OrElse;

		private void VisitLeftAndRight(ExpressionType nodeType, Expression left, Expression right)
		{
			if (IsAddBrackets(nodeType))
				_exp.SqlText += "(";
			base.Visit(left);
			if (_dictOperator.TryGetValue(nodeType, out string operat))
				_exp.SqlText += operat;
			else
				_exp.SqlText += nodeType.ToString();
			base.Visit(right);
			if (IsAddBrackets(nodeType))
				_exp.SqlText += ")";
		}

		private bool TransferEnum(BinaryExpression node)
		{
			var arr = new[] { node.Left, node.Right };
			if (arr.Count(a => a.NodeType == ExpressionType.Convert) == 1)
			{
				UnaryExpression convertExpression = null;
				Expression otherExpression = null;
				foreach (var item in arr)
				{
					if (item is UnaryExpression ue)
					{
						var type = ue.Operand.Type.GetOriginalType();
						if (type.IsEnum && ue.Type.GetOriginalType() == typeof(int))
							convertExpression = ue;
					}
					else
						otherExpression = item;
				}
				if (convertExpression != null
					&& !IsDbMember(otherExpression, out MemberExpression _))
				{
					VisitLeftAndRight(node.NodeType, convertExpression,
						Expression.Constant(Enum.ToObject(convertExpression.Operand.Type, GetExpressionInvokeResultObject(otherExpression)), convertExpression.Operand.Type));
					return true;
				}
				else
					_isGetConvertException = true;
			}
			return false;
		}

		private void SetExpressionInvokeResultParameter(Expression node)
		{
			var value = GetExpressionInvokeResultObject(node);
			if (!ChecktAndSetNullValue(value))
				SetParameter(value);
		}

		private void MethodContaionsHandler(MethodCallExpression node)
		{
			int support = 0;
			void AddOperator() => _exp.SqlText += _currentLambdaNodeType == ExpressionType.Not ? " <> " : " = ";
			bool AnalysisDbField(List<Expression> expression, int i)
			{
				var expType = expression[i].Type;
				if (expType.IsArray || expType.FullName.StartsWith("System.Collections.Generic.List`1"))
				{
					if (i == 0)
					{
						AnalysisDbField(expression, i + 1);
						AddOperator();
					}
					_exp.SqlText += _currentLambdaNodeType == ExpressionType.Not ? "ALL(" : "ANY(";
					base.Visit(expression[i]);
					_exp.SqlText += ")";
					support++;
					return true;
				}
				base.Visit(expression[i]);
				return false;
			}
			var argList = node.Arguments.ToList();
			if (node.Object != null)
				argList.Insert(0, node.Object);
			if (!AnalysisDbField(argList, 0))
			{
				AddOperator();
				AnalysisDbField(argList, 1);
			}
			if (support == 0)
				throw new NotSupportedException("Contains method property only supported 'new T[]' and 'list<T>'");

		}

		/// <summary>
		/// string.contains/startswith/endswith
		/// </summary>
		/// <param name="node"></param>
		/// <param name="key"></param>
		/// <param name="fieldKey"></param>
		/// <returns></returns>
		private bool StringLikeCalling(MethodCallExpression node, string key, string fieldKey)
		{
			if (node.Object == null) return false;
			if (node.Object.Type == typeof(string))
			{
				Visit(node.Object);

				_exp.SqlText += _currentLambdaNodeType == ExpressionType.Not ? " NOT" : string.Empty;
				if (node.Arguments.Count == 2 && node.Arguments[1].ToString().EndsWith("IgnoreCase"))
					_exp.SqlText += " ILIKE ";
				else
					_exp.SqlText += " LIKE ";

				_methodStringContainsFormat = IsDbMember(node.Object, out MemberExpression _) ? key : fieldKey;
				base.Visit(node.Arguments[0]);
				_methodStringContainsFormat = null;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 检查并设置null值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool ChecktAndSetNullValue(object value)
		{
			if (value != null) return false;
			if (_currentLambdaNodeType == ExpressionType.Equal)
				_exp.SqlText = string.Concat(_exp.SqlText.Trim().TrimEnd('='), "IS NULL");
			else if (_currentLambdaNodeType == ExpressionType.NotEqual)
				_exp.SqlText = string.Concat(_exp.SqlText.Trim().TrimEnd('>', '<'), "IS NOT NULL");
			return true;
		}

		/// <summary>
		/// 设置成员的值
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="convertType"></param>
		private void SetMemberValue(Expression expression, Type convertType)
		{
			var obj = GetExpressionInvokeResultObject(expression);
			if (ChecktAndSetNullValue(obj)) return;
			var value = Convert.ChangeType(obj, convertType.GetOriginalType());
			SetParameter(value);
		}

		/// <summary>
		/// 输出表达式的值
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		private object GetExpressionInvokeResultObject(Expression expression)
		{
			return Expression.Lambda(expression).Compile().DynamicInvoke();
		}

		/// <summary>
		/// 输出表达式的值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		private T GetExpressionInvokeResult<T>(Expression expression)
		{
			return Expression.Lambda<Func<T>>(Expression.Convert(expression, typeof(T))).Compile().Invoke();
		}

		/// <summary>
		/// 设置参数
		/// </summary>
		/// <param name="value"></param>
		private void SetParameter(object value)
		{
			var index = EntityHelper.ParamsIndex;
			if (_methodStringContainsFormat != null)
				value = string.Format(_methodStringContainsFormat, value);
			//_exp.Paras.Add(_dbExecute.ConnectionOptions.GetDbParameter(index, value));
			_exp.SqlText += string.Concat("@", index);
		}

		/// <summary>
		/// 递归member表达式, 针对optional字段, 从 a.xxx.Value->a.xxx
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private MemberExpression MemberVisitor(MemberExpression node)
		{
			if (node.NodeType == ExpressionType.MemberAccess && node.Expression is MemberExpression me)
				return MemberVisitor(me);
			return node;
		}

		/// <summary>
		/// 是否数据库成员
		/// </summary>
		/// <param name="node"></param>
		/// <param name="dbMember">a.xxx成员</param>
		/// <returns></returns>
		private bool IsDbMember(MemberExpression node, out MemberExpression dbMember)
		{
			dbMember = MemberVisitor(node);
			return dbMember.Expression != null && dbMember.Expression.NodeType == ExpressionType.Parameter;
		}

		/// <summary>
		/// 是否数据库成员
		/// </summary>
		/// <param name="node"></param>
		/// <param name="dbMember">a.xxx成员</param>
		/// <returns></returns>
		private bool IsDbMember(Expression node, out MemberExpression dbMember)
		{
			dbMember = null;
			if (node == null) return false;
			return node is MemberExpression mbe && IsDbMember(mbe, out dbMember);
		}
		#endregion
	}
}
