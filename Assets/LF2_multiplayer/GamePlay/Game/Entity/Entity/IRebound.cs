namespace LF2.Client
{
    interface IRebound
    {
        public void Rebound();

        public bool IsReboundable();
        
    }
    interface IProjectileHurtBoxType
    {
        public ProjectileBoxType Type {get;}
    }
}