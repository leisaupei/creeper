namespace Creeper.Driver
{
	internal class CreeperConnectionOption : ICreeperConnectionOption
	{
		public CreeperConnectionOption(CreeperConnection main, CreeperConnection[] secondary)
		{
			Main = main;
			Secondary = secondary;
		}

		/// <summary>
		/// 主库
		/// </summary>
		public ICreeperConnection Main { get; }

		/// <summary>
		/// 从库
		/// </summary>
		public ICreeperConnection[] Secondary { get; }

	}
}
