namespace Creeper.Driver
{
	public interface ICreeperConnectionOption
    {
        /// <summary>
        /// 主库
        /// </summary>
        ICreeperConnection Main { get; }

        /// <summary>
        /// 从库
        /// </summary>
        ICreeperConnection[] Secondary { get; }

    }
}
