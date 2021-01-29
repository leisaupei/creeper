namespace Creeper.Driver
{
	public interface ICreeperDbOption
	{
		/// <summary>
		/// 主库
		/// </summary>
		public ICreeperDbConnectionOption Main { get; }

		/// <summary>
		/// 从库
		/// </summary>
		public ICreeperDbConnectionOption[] Secondary { get; }
	}
}
