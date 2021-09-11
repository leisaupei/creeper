using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Creeper.MySql.Types
{
	public abstract class MySqlGeometry
	{
		static readonly Regex _regexMySqlPoint = new Regex(@"\s*(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s*");
		static readonly Regex _regexSplit1 = new Regex(@"\)\s*,\s*\(");
		static readonly Regex _regexSplit2 = new Regex(@"\)\s*\)\s*,\s*\(\s*\(");
		static readonly Regex _regexSplit3 = new Regex(@"\s*,\s*(?=[a-zA-Z])");
		public int? SRID { get; set; }

		public override string ToString() => this switch
		{
			null => null,
			MySqlPoint p => $"{(SRID != null && SRID != 0 ? $"SRID={SRID};" : "")}POINT({p.X} {p.Y})",
			MySqlLineString ls => ls.Count > 0 ? $"LINESTRING({string.Join(",", ls.Select(a => $"{a.X} {a.Y}"))})" : null,
			MySqlPolygon pg => pg.Any() ? $"POLYGON(({string.Join("),(", pg.Select(c => string.Join(",", c.Select(a => $"{a.X} {a.Y}"))))}))" : null,
			MySqlMultiPoint mp => mp.Count > 0 ? $"MULTIPOINT({string.Join(",", mp.Select(a => $"{a.X} {a.Y}"))})" : null,
			MySqlMultiLineString mls => mls.Count > 0 ? $"MULTILINESTRING(({string.Join("),(", mls.Select(c => string.Join(",", c.Select(a => $"{a.X} {a.Y}"))))}))" : null,
			MySqlMultiPolygon mp => mp.Any() ? $"MULTIPOLYGON((({string.Join(")),((", mp.Select(d => string.Join("),(", d.Select(c => string.Join(",", c.Select(a => $"{a.X} {a.Y}"))))))})))" : null,
			MySqlGeometryCollection gc => gc.Any() ? $"GEOMETRYCOLLECTION({string.Join(",", gc.Select(a => a.ToString()))})" : null,
			_ => base.ToString(),
		};

		public static MySqlGeometry Parse(string wkt)
		{
			if (string.IsNullOrWhiteSpace(wkt)) return null;
			wkt = wkt.Trim();
			if (wkt.StartsWith("point", StringComparison.CurrentCultureIgnoreCase))
				return ParsePoint(wkt[6..(wkt.Length - 1)]);

			else if (wkt.StartsWith("linestring", StringComparison.CurrentCultureIgnoreCase))
				return new MySqlLineString(ParsePoints(wkt[11..(wkt.Length - 1)]));

			else if (wkt.StartsWith("polygon", StringComparison.CurrentCultureIgnoreCase))
				return new MySqlPolygon(ParsePolygon(wkt[8..(wkt.Length - 1)]));

			else if (wkt.StartsWith("multipoint", StringComparison.CurrentCultureIgnoreCase))
				return new MySqlMultiPoint(ParsePoints(wkt[11..(wkt.Length - 1)]));

			else if (wkt.StartsWith("multilinestring", StringComparison.CurrentCultureIgnoreCase))
				return new MySqlMultiLineString(ParseMultiLineString(wkt[16..(wkt.Length - 1)]));

			else if (wkt.StartsWith("multipolygon", StringComparison.CurrentCultureIgnoreCase))
				return new MySqlMultiPolygon(ParseMultiPolygon(wkt[13..(wkt.Length - 1)]));

			else if (wkt.StartsWith("geometrycollection", StringComparison.CurrentCultureIgnoreCase))
				return new MySqlGeometryCollection(ParseGeometryCollection(wkt[19..(wkt.Length - 1)]));

			throw new NotImplementedException($"MySqlGeometry.Parse 未实现 \"{wkt}\"");
		}

		static MySqlPoint ParsePoint(string str)
		{
			var m = _regexMySqlPoint.Match(str);
			if (m.Success == false) return null;
			return new MySqlPoint(double.TryParse(m.Groups[1].Value, out var d) ? d : 0, double.TryParse(m.Groups[2].Value, out d) ? d : 0);
		}

		static MySqlPoint[] ParsePoints(string str) => _regexMySqlPoint.Matches(str).Select(a => new MySqlPoint(double.TryParse(a.Groups[1].Value, out var d) ? d : 0, double.TryParse(a.Groups[2].Value, out d) ? d : 0)).ToArray();

		static MySqlPoint[][] ParsePolygon(string str) => _regexSplit1.Split(str).Select(s => ParsePoints(s)).Where(a => a.Length > 1 && a.First().Equals(a.Last())).ToArray();

		static MySqlLineString[] ParseMultiLineString(string str) => _regexSplit1.Split(str).Select(s => new MySqlLineString(ParsePoints(s))).ToArray();

		static MySqlPolygon[] ParseMultiPolygon(string str) => _regexSplit2.Split(str).Select(s => new MySqlPolygon(ParsePolygon(s))).ToArray();

		static MySqlGeometry[] ParseGeometryCollection(string str) => _regexSplit3.Split(str).Select(s => Parse(s)).ToArray();
	}

	public class MySqlGeometryCollection : MySqlGeometry, IEquatable<MySqlGeometryCollection>, IEnumerable<MySqlGeometry>
	{
		readonly MySqlGeometry[] _geometries;

		public int Count => _geometries.Length;

		public MySqlGeometry this[int index] => _geometries[index];

		public IEnumerator<MySqlGeometry> GetEnumerator() => ((IEnumerable<MySqlGeometry>)_geometries).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlGeometryCollection(MySqlGeometry[] geometries) => _geometries = geometries;

		public MySqlGeometryCollection(IEnumerable<MySqlGeometry> geometries) => _geometries = geometries.ToArray();

		public bool Equals(MySqlGeometryCollection other)
		{
			if (other is null || _geometries.Length != other._geometries.Length)
				return false;
			for (var i = 0; i < _geometries.Length; i++)
				if (!_geometries[i].Equals(other._geometries[i]))
					return false;
			return true;
		}

		public override bool Equals(object obj) => obj is MySqlGeometryCollection collection && Equals(collection);

		public static bool operator ==(MySqlGeometryCollection left, MySqlGeometryCollection right) => left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlGeometryCollection left, MySqlGeometryCollection right) => !(left == right);

		public override int GetHashCode() => HashCode.Combine(SRID, _geometries);
	}

	public class MySqlLineString : MySqlGeometry, IEnumerable<MySqlPoint>, IEquatable<MySqlLineString>
	{
		readonly MySqlPoint[] _points;

		public IEnumerator<MySqlPoint> GetEnumerator() => ((IEnumerable<MySqlPoint>)_points).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlLineString(IEnumerable<MySqlPoint> points) => _points = points.ToArray();

		public MySqlLineString(MySqlPoint[] points) => _points = points;

		public int Count => _points.Length;

		public bool Equals(MySqlLineString other)
		{
			if (other is null || _points.Length != other._points.Length)
				return false;
			for (var i = 0; i < _points.Length; i++)
				if (!_points[i].Equals(other._points[i]))
					return false;
			return true;
		}

		public override bool Equals(object obj) => Equals(obj as MySqlLineString);

		public override int GetHashCode() => HashCode.Combine(SRID, _points);

		public static bool operator ==(MySqlLineString left, MySqlLineString right) => left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlLineString left, MySqlLineString right) => !(left == right);

	}

	public class MySqlMultiLineString : MySqlGeometry, IEnumerable<MySqlLineString>, IEquatable<MySqlMultiLineString>
	{
		readonly MySqlLineString[] _lineStrings;

		internal MySqlMultiLineString(MySqlPoint[][] pointArray)
		{
			_lineStrings = new MySqlLineString[pointArray.Length];
			for (var i = 0; i < pointArray.Length; i++)
				_lineStrings[i] = new MySqlLineString(pointArray[i]);
		}

		public int Count => _lineStrings.Length;
		public IEnumerator<MySqlLineString> GetEnumerator() => ((IEnumerable<MySqlLineString>)_lineStrings).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlMultiLineString(MySqlLineString[] linestrings)
			=> _lineStrings = linestrings;

		public MySqlMultiLineString(IEnumerable<MySqlLineString> linestrings)
			=> _lineStrings = linestrings.ToArray();


		public MySqlLineString this[int index] => _lineStrings[index];

		public bool Equals(MySqlMultiLineString other)
		{
			if (other is null || _lineStrings.Length != other._lineStrings.Length)
				return false;

			for (var i = 0; i < _lineStrings.Length; i++)
				if (_lineStrings[i] != other._lineStrings[i])
					return false;
			return true;
		}

		public override bool Equals(object obj) => obj is MySqlMultiLineString multiLineString && Equals(multiLineString);

		public static bool operator ==(MySqlMultiLineString x, MySqlMultiLineString y) => x is null ? y is null : x.Equals(y);

		public static bool operator !=(MySqlMultiLineString x, MySqlMultiLineString y) => !(x == y);

		public override int GetHashCode() => HashCode.Combine(SRID, _lineStrings);
	}

	public class MySqlMultiPoint : MySqlGeometry, IEnumerable<MySqlPoint>, IEquatable<MySqlMultiPoint>
	{
		readonly MySqlPoint[] _points;

		public MySqlMultiPoint(IEnumerable<MySqlPoint> points) => _points = points.ToArray();

		public MySqlMultiPoint(MySqlPoint[] points) => _points = points;

		public int Count => _points.Length;

		public bool Equals(MySqlMultiPoint other)
		{
			if (other == null || _points.Length != other._points.Length)
				return false;

			for (var i = 0; i < _points.Length; i++)
				if (!_points[i].Equals(other._points[i]))
					return false;

			return true;
		}

		public static bool operator ==(MySqlMultiPoint left, MySqlMultiPoint right) => left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlMultiPoint p1, MySqlMultiPoint p2) => !(p1 == p2);

		public IEnumerator<MySqlPoint> GetEnumerator() => ((IEnumerable<MySqlPoint>)_points).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override bool Equals(object obj) => Equals(obj as MySqlPolygon);

		public override int GetHashCode() => HashCode.Combine(SRID, _points);
	}

	public class MySqlMultiPolygon : MySqlGeometry, IEnumerable<MySqlPolygon>, IEquatable<MySqlMultiPolygon>
	{

		readonly MySqlPolygon[] _polygons;

		public IEnumerator<MySqlPolygon> GetEnumerator() => ((IEnumerable<MySqlPolygon>)_polygons).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlMultiPolygon(MySqlPolygon[] polygons) => _polygons = polygons;

		public MySqlMultiPolygon(IEnumerable<MySqlPolygon> polygons) => _polygons = polygons.ToArray();

		public bool Equals(MySqlMultiPolygon other)
		{
			if (other is null || _polygons.Length != other._polygons.Length)
				return false;
			for (var i = 0; i < _polygons.Length; i++)
				if (_polygons[i] != other._polygons[i]) return false;
			return true;
		}

		public override bool Equals(object obj) => obj is MySqlMultiPolygon polygon && Equals(polygon);

		public override int GetHashCode() => HashCode.Combine(SRID, _polygons);

		public static bool operator ==(MySqlMultiPolygon left, MySqlMultiPolygon right) => left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlMultiPolygon left, MySqlMultiPolygon right) => !(left == right);

	}

	public class MySqlPoint : MySqlGeometry, IEquatable<MySqlPoint>
	{
		public double X { get; }

		public double Y { get; }

		public MySqlPoint(double x, double y) { X = x; Y = y; }

		public bool Equals(MySqlPoint p) => X == p.X && Y == p.Y;

		public override bool Equals(object obj) => obj is MySqlPoint p && Equals(p);

		public override int GetHashCode() => HashCode.Combine(X, Y);

		public static bool operator ==(MySqlPoint left, MySqlPoint right) => Equals(left, right);

		public static bool operator !=(MySqlPoint left, MySqlPoint right) => !Equals(left, right);
	}

	public class MySqlPolygon : MySqlGeometry, IEnumerable<IEnumerable<MySqlPoint>>, IEquatable<MySqlPolygon>
	{
		readonly MySqlPoint[][] _points;

		public MySqlPolygon(IEnumerable<IEnumerable<MySqlPoint>> points) : this(points.Select(a => a.ToArray()).ToArray()) { }

		public MySqlPolygon(MySqlPoint[][] points)
		{
			if (!points?.Any() ?? true || points.Any(a => !a.Any()))
				throw new ArgumentNullException(nameof(points));

			if (points.Any(a => a.First() != a.Last() || a.Length < 4))
				throw new ArgumentException(nameof(points));

			_points = points;
		}
		public bool Equals(MySqlPolygon other)
		{
			if (other == null || _points.Length != other._points.Length)
				return false;

			for (var i = 0; i < _points.Length; i++)
				for (int j = 0; j < _points[i].Length; j++)
					if (!_points[i][j].Equals(other._points[i][j]))
						return false;

			return true;
		}

		public static bool operator ==(MySqlPolygon left, MySqlPolygon right) => left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlPolygon left, MySqlPolygon right) => !(left == right);


		public override bool Equals(object obj) => Equals(obj as MySqlPolygon);

		public override int GetHashCode() => HashCode.Combine(SRID, _points);

		public IEnumerator<IEnumerable<MySqlPoint>> GetEnumerator() => ((IEnumerable<IEnumerable<MySqlPoint>>)_points).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
