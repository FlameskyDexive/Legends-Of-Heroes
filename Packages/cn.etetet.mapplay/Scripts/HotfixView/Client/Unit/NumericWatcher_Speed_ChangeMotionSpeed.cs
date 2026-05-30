namespace ET.Client
{
	/// <summary>
	/// 客户端监视speed数值变化，改变血条值
	/// </summary>
	[NumericWatcher(SceneType.Current, NumericType.Speed)]
	public class NumericWatcher_Speed_ChangeMotionSpeed : INumericWatcher
	{
		public void Run(Unit unit, NumbericChange args)
		{
			// 无动画的单位(如球球大作战的球)没有 AnimatorComponent,或视图(异步加载)还没装配完 → 跳过,避免空引用。
			AnimatorComponent animatorComponent = unit.GetComponent<AnimatorComponent>();
			if (animatorComponent == null)
			{
				return;
			}
			animatorComponent.SetFloat(nameof(MotionType.MoveSpeed), args.New / 1000f);
		}
	}
}
