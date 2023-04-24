namespace ET.Client
{
	/// <summary>
	/// 客户端监视hp数值变化，改变血条值
	/// </summary>
	[NumericWatcher(SceneType.Current, NumericType.Hp)]
	public class NumericWatcher_HpChange : INumericWatcher
	{
		public void Run(Unit unit, EventType.NumbericChange args)
		{
            if (unit.Type == UnitType.Player)
            {
                unit.GetComponent<GameObjectComponent>()?.RefreshScale();
            }
		}
	}
}
