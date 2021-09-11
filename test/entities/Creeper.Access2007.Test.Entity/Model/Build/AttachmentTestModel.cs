using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	[CreeperTable("[AttachmentTest]")]
	public partial class AttachmentTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update)]
		public string TypeTest { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update)]
		public string Archives { get; set; }
	}
}
