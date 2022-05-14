namespace GameServer.Services
{   
    class DBService : Common.Singleton<DBService>
    {
        ExtremeWorldEntities entities;
        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }
    
        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="async">是否异步</param>
        public void Save(bool async = false)
        {
            if (async){ entities.SaveChangesAsync(); }
            else { entities.SaveChanges();}
        }

    }
}
