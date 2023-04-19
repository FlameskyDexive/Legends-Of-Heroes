namespace ET
{
	// 这个可弄个配置表生成
    public static class NumericType
    {
	    public const int Max = 10000;

	    public const int Speed = 1000;
	    public const int SpeedBase = Speed * 10 + 1;
	    public const int SpeedAdd = Speed * 10 + 2;
	    public const int SpeedPct = Speed * 10 + 3;
	    public const int SpeedFinalAdd = Speed * 10 + 4;
	    public const int SpeedFinalPct = Speed * 10 + 5;

	    public const int Hp = 1001;
	    public const int HpBase = Hp * 10 + 1;
        public const int HpAdd = Hp * 10 + 2;
        public const int HpPct = Hp * 10 + 3;
        public const int HpFinalAdd = Hp * 10 + 4;
        public const int HpFinalPct = Hp * 10 + 5;

        public const int MaxHp = 1002;
	    public const int MaxHpBase = MaxHp * 10 + 1;
	    public const int MaxHpAdd = MaxHp * 10 + 2;
	    public const int MaxHpPct = MaxHp * 10 + 3;
	    public const int MaxHpFinalAdd = MaxHp * 10 + 4;
	    public const int MaxHpFinalPct = MaxHp * 10 + 5;

	    public const int AOI = 1003;
	    public const int AOIBase = AOI * 10 + 1;
	    public const int AOIAdd = AOI * 10 + 2;
	    public const int AOIPct = AOI * 10 + 3;
	    public const int AOIFinalAdd = AOI * 10 + 4;
	    public const int AOIFinalPct = AOI * 10 + 5;


        public const int Attack = 1011;         //攻击力
        public const int AttackBase = Attack * 10 + 1;
        public const int AttackAdd = Attack * 10 + 2;
        public const int AttackPct = Attack * 10 + 3;
        public const int AttackFinalAdd = Attack * 10 + 4;
        public const int AttackFinalPct = Attack * 10 + 5;
        
        
        //////////子弹属性相关//////////////
        public const int BulletRadius = 1101;	//子弹半径
        public const int BulletLife = 1101;	//子弹周期
    }
}
