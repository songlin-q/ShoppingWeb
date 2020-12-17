using ShoppingModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace ShoppingDAL
{
    public class BaseDAL<T> where T: class,new()
    {

        stu_ach_managenmentEntities db = new stu_ach_managenmentEntities();

        #region 1.0 新增 实体 +int Add(T model)
        /// <summary>
        /// 新增 实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(T model)
        {

            db.Set<T>().Add(model);


            int res = db.SaveChanges();


            return res;


        }
        /// <summary>
        /// 批量加入多个
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public int AddAll(List<T> models)
        {
            //try
            //{
            models.ForEach(model => { db.Set<T>().Add(model); });

            int res = db.SaveChanges();//保存成功后，会将自增的id设置给 model的 主键属性，并返回受影响行数
            return res;
    

        }



        private bool EventTypeFilter(System.Reflection.PropertyInfo p)
        {
            var attribute = Attribute.GetCustomAttribute(p,
                typeof(AssociationAttribute)) as AssociationAttribute;

            if (attribute == null) return true;
            if (attribute.IsForeignKey == false) return true;

            return false;
        }

        private object GetPropertyValue(object o)
        {
            if (o == null)
                return DBNull.Value;
            return o;
        }

        #endregion

        #region 2.0 根据 id 删除 +int Del(T model)
        /// <summary>
        /// 根据 id 删除
        /// </summary>
        /// <param name="model">包含要删除id的对象</param>
        /// <returns></returns>
        public int Del(T model)
        {
            db.Set<T>().Attach(model);
            db.Set<T>().Remove(model);
            return db.SaveChanges();
        }
        #endregion



        #region 4.0 修改 +int Modify(T model, params string[] proNames)
        /// <summary>
        /// 4.0 修改，如：
        /// T u = new T() { uId = 1, uLoginName = "asdfasdf" };
        /// this.Modify(u, "uLoginName");
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="proNames">要修改的 属性 名称</param>
        /// <returns></returns>
        public int Modify(T model, params string[] proNames)
        {

            //4.1将 对象 添加到 EF中
            DbEntityEntry entry = db.Entry<T>(model);
            //4.2先设置 对象的包装 状态为 Unchanged
            entry.State = System.Data.Entity.EntityState.Unchanged;
            //4.3循环 被修改的属性名 数组
            foreach (string proName in proNames)
            {
                //4.4将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                entry.Property(proName).IsModified = true;
            }
            //4.4一次性 生成sql语句到数据库执行

            return db.SaveChanges();
   
        }





        /// <summary>
        /// EF高效批量更新方法
        /// </summary>
        /// <param name="whereLambda">需要更新的数据源</param>
        /// <param name="upModel">要更新的字段</param>
        /// <returns></returns>
        public int ModifyBy_GaoXiao(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> upModel)
        {
            return db.Set<T>().Where(whereLambda).Update(upModel);
        }




        public int Modify_List(List<T> modelList, params string[] proNames)
        {
            modelList.ForEach(model =>
            {
                //4.1将 对象 添加到 EF中
                DbEntityEntry entry = db.Entry<T>(model);
                //4.2先设置 对象的包装 状态为 Unchanged
                entry.State = System.Data.Entity.EntityState.Unchanged;
                //4.3循环 被修改的属性名 数组
                foreach (string proName in proNames)
                {
                    //4.4将每个 被修改的属性的状态 设置为已修改状态;后面生成update语句时，就只为已修改的属性 更新
                    entry.Property(proName).IsModified = true;
                }
                //4.4一次性 生成sql语句到数据库执行
            });
            return db.SaveChanges();
        }





        #endregion

        #region 4.0 批量修改 +int Modify(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        /// <summary>
        /// 4.0 批量修改
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="proNames">要修改的 属性 名称</param>
        /// <returns></returns>
        public int ModifyBy(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        {
            //4.1查询要修改的数据
            List<T> listModifing = db.Set<T>().Where(whereLambda).ToList();

            //获取 实体类 类型对象
            Type t = typeof(T); // model.GetType();
            //获取 实体类 所有的 公有属性
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //创建 实体属性 字典集合
            Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
            //将 实体属性 中要修改的属性名 添加到 字典集合中 键：属性名  值：属性对象
            proInfos.ForEach(p =>
            {
                if (modifiedProNames.Contains(p.Name))
                {
                    dictPros.Add(p.Name, p);
                }
            });

            //4.3循环 要修改的属性名
            foreach (string proName in modifiedProNames)
            {
                //判断 要修改的属性名是否在 实体类的属性集合中存在
                if (dictPros.ContainsKey(proName))
                {
                    //如果存在，则取出要修改的 属性对象
                    PropertyInfo proInfo = dictPros[proName];
                    //取出 要修改的值
                    object newValue = proInfo.GetValue(model, null); //object newValue = model.uName;

                    //4.4批量设置 要修改 对象的 属性
                    foreach (T usrO in listModifing)
                    {
                        //为 要修改的对象 的 要修改的属性 设置新的值
                        proInfo.SetValue(usrO, newValue, null); //usrO.uName = newValue;
                    }
                }
            }
            //4.4一次性 生成sql语句到数据库执行
            return db.SaveChanges();
        }
        #endregion

        #region 5.0 根据条件查询 +List<T> GetListBy(Expression<Func<T,bool>> whereLambda)
        /// <summary>
        /// 5.0 根据条件查询 +List<T> GetListBy(Expression<Func<T,bool>> whereLambda)
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public List<T> GetListBy(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda).ToList();
        }
        public T GetFirstOrDefaultBy(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda).FirstOrDefault();
        }

        public T GetLastOrDefaulttBy(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda).LastOrDefault();
        }

        public List<object> GetListSelectObjBy(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> selectLambda)
        {
            return db.Set<T>().Where(whereLambda).Select(selectLambda).ToList();
        }

        #endregion

        #region 5.1.1 返回查询实体(还未查询数据库)
        /// <summary>
        /// 返回查询实体(还未查询数据库)
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public System.Linq.IQueryable<T> GetIQueryableObjBy(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda);
        }
        public System.Linq.IQueryable<T> GetIQueryableObjBy_AsNoTracking(Expression<Func<T, bool>> whereLambda)
        {
            var queryable = db.Set<T>().AsNoTracking().Where(whereLambda);
          
            return queryable;
        }
        #endregion


        #region 5.3 根据条件返回数量
        /// <summary>
        /// 5.3 根据条件返回数量
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public int GetCountBy(Expression<Func<T, bool>> whereLambda)
        {
            try
            {
                return db.Set<T>().AsNoTracking().Where(whereLambda).Count();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region 5.1 根据条件 排序 和查询 + List<T> GetListBy<TKey>
        /// <summary>
        /// 5.1 根据条件 排序 和查询
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件 lambda表达式</param>
        /// <param name="orderLambda">排序条件 lambda表达式</param>
        /// <returns></returns>
        public List<T> GetListBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda)
        {
            return db.Set<T>().Where(whereLambda).OrderBy(orderLambda).ToList();
        }
        #endregion

        #region 6.0 分页查询 + List<T> GetPagedList<TKey>
        /// <summary>
        /// 6.0 分页查询 + List<T> GetPagedList<TKey>
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public List<T> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            return db.Set<T>().AsNoTracking().Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 6.03分页查询 + List<T> GetPagedList<TKey>
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public System.Linq.IQueryable<T> GetPagedList_IQueryableObjBy<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            return db.Set<T>().Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        #endregion

        #region 6.02 分页查询 逆序 + List<T> GetPagedList<TKey>
        /// <summary>
        /// 6.2 分页查询 逆序 + List<T> GetPagedList<TKey>
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public List<T> GetPagedListOrderByDescending<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            return db.Set<T>().AsNoTracking().Where(whereLambda).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        /// <summary>
        /// 6.04 分页查询 逆序 + List<T> GetPagedList<TKey>
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public System.Linq.IQueryable<T> GetPagedListOrderByDescending_IQueryableObjBy<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            return db.Set<T>().Where(whereLambda).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        #endregion



        #region 6.1分页查询 带输出 +List<T> GetPagedList<TKey>
        /// <summary>
        /// 6.1分页查询 带输出
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="rowCount"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderBy"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public List<T> GetPagedList<TKey>(int pageIndex, int pageSize, ref int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy, bool isAsc = true)
        {
            rowCount = db.Set<T>().Where(whereLambda).Count();
            //1.查询分页数据
            if (isAsc)
            {
                return db.Set<T>().OrderBy(orderBy).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            //2.查询总行数
            else
            {
                return db.Set<T>().OrderByDescending(orderBy).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        #endregion


        //-----------------------------------------------------------------
        #region 7.0 执行普通sql语句 + int ExcuteSql(string strSql, params object[] paras);
        /// <summary>
        /// 执行普通sql语句
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public virtual int ExcuteSql(string strSql, params object[] paras)
        {
            return db.Database.ExecuteSqlCommand(strSql, paras);
        }
        public virtual List<T> ExcuteQuerySql(string strSql, params object[] paras)
        {
            return db.Database.SqlQuery<T>(strSql, paras).ToList();
        }
        public virtual int ExcuteQuerySqlForCount(string strSql, params object[] paras)
        {
            return db.Database.SqlQuery<int>(strSql, paras).FirstOrDefault();
        }
        public virtual decimal ExcuteQuerySqlForCountForObj(string strSql, params object[] paras)
        {
            return db.Database.SqlQuery<decimal>(strSql, paras).FirstOrDefault();
        }
        public virtual object ExcuteQuerySqlForCountForObj_Proc(string strSql, params object[] paras)
        {
            return db.Database.SqlQuery<object>(strSql, paras).ToList();
        }
        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual DataTable SqlQueryForDataTatable(string strSql, params SqlParameter[] parameters)
        {
            SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = db.Database.Connection.ConnectionString;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = strSql;
            if (parameters.Length > 0)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            conn.Close();
            return table;
        }
        #endregion



    }
}
