using ShoppingDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingBLL
{
    public abstract class BaseBLL<T> where T : class, new()
    {


        public BaseBLL()
        {
            SetDAL();
        }
        /// <summary>
        /// 由子类实现，为 业务父类 里的 数据接口对象 设置 值！
        /// </summary>
        public abstract void SetDAL();

        protected BaseDAL<T> idal;

        //2.增删改查方法
        #region 1.0 新增 实体 +int Add(T model)
        /// <summary>
        /// 新增 实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(T model)
        {
            return idal.Add(model);
        }
        /// <summary>
        /// 批量加入多个
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public int AddAll(List<T> models)
        {
            return idal.AddAll(models);
        }


        #endregion

        #region 2.0 根据 用户 id 删除 +int Del(int uId)
        /// <summary>
        /// 根据 用户 id 删除
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public int Del(T model)
        {
            return idal.Del(model);
        }
        #endregion



        #region 4.0 修改 +int Modify(T model, params string[] proNames)
        /// <summary>
        /// 4.0 批量修改，建议使用ModifyBy_GaoXiao
        /// </summary>
        /// <param name="model">要修改的实体对象</param>
        /// <param name="proNames">要修改的 属性 名称</param>
        /// <returns></returns>
        public int Modify(T model, params string[] proNames)
        {
            return idal.Modify(model, proNames);
        }





        /// <summary>
        /// EF高效批量更新方法
        /// </summary>
        /// <param name="whereLambda">需要更新的数据源</param>
        /// <param name="upModel">要更新的字段</param>
        /// <returns></returns>
        public int ModifyBy_GaoXiao(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> upModel)
        {
            return idal.ModifyBy_GaoXiao(whereLambda, upModel);
        }

        public int Modify_List(List<T> modelList, params string[] proNames)
        {
            return idal.Modify_List(modelList, proNames);
        }



        #endregion

        #region  4.0 批量修改 +int Modify(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        /// <summary>
        ///  4.0 批量修改 +int Modify(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="whereLambda"></param>
        /// <param name="modifiedProNames"></param>
        /// <returns></returns>
        public int ModifyBy(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames)
        {
            return idal.ModifyBy(model, whereLambda, modifiedProNames);
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
            return idal.GetListBy(whereLambda);
        }
        //public List<T> GetListBy(Expression<Func<T, bool>> whereLambda)
        //{
        //    return idal.GetListBy(whereLambda);
        //}
        public T GetFirstOrDefaultBy(Expression<Func<T, bool>> whereLambda)
        {
            return idal.GetFirstOrDefaultBy(whereLambda);
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
            return idal.GetListBy(whereLambda, orderLambda);
        }
        #endregion

        #region 5.1.1 返回查询实体(还未查询数据库)
        /// <summary>
        /// 以后不要再用此方法，请用GetIQueryableObjBy_AsNoTracking
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public System.Linq.IQueryable<T> GetIQueryableObjBy(Expression<Func<T, bool>> whereLambda)
        {
            return idal.GetIQueryableObjBy(whereLambda);
        }
        public System.Linq.IQueryable<T> GetIQueryableObjBy_AsNoTracking(Expression<Func<T, bool>> whereLambda)
        {
            return idal.GetIQueryableObjBy_AsNoTracking(whereLambda);
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
            return idal.GetCountBy(whereLambda);
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
            return idal.GetPagedList(pageIndex, pageSize, whereLambda, orderBy);
        }
        /// <summary>
        /// 6.03分页查询 + List<T> GetPagedList_IQueryableObjBy优化代码，查询结果需要ToList<TKey>
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public System.Linq.IQueryable<T> GetPagedList_IQueryableObjBy<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            return idal.GetPagedList_IQueryableObjBy(pageIndex, pageSize, whereLambda, orderBy);
        }
        #endregion

        #region 6.02 分页查询 逆序 + List<T> GetPagedList<TKey>
        /// <summary>
        /// 6.02 分页查询 逆序 + List<T> GetPagedList<TKey>
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public List<T> GetPagedListOrderByDescending<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            return idal.GetPagedListOrderByDescending(pageIndex, pageSize, whereLambda, orderBy);
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
            return idal.GetPagedListOrderByDescending_IQueryableObjBy(pageIndex, pageSize, whereLambda, orderBy);
        }
        #endregion


        /// <summary>
        /// 执行普通sql语句
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public virtual int ExcuteSql(string strSql, params object[] paras)
        {
            return idal.ExcuteSql(strSql, paras);
        }
        public virtual List<T> ExcuteQuerySql(string strSql, params object[] paras)
        {
            return idal.ExcuteQuerySql(strSql, paras);
        }
        /// <summary>
        /// 通过sql语句，获取返回数量
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public virtual int ExcuteQuerySqlForCount(string strSql, params object[] paras)
        {
            return idal.ExcuteQuerySqlForCount(strSql, paras);

        }

        public virtual decimal ExcuteQuerySqlForCountForObj(string strSql, params object[] paras)
        {
            return idal.ExcuteQuerySqlForCountForObj(strSql, paras);
        }
        public virtual object ExcuteQuerySqlForCountForObj_Proc(string strSql, params object[] paras)
        {
            return idal.ExcuteQuerySqlForCountForObj_Proc(strSql, paras);
        }
        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual DataTable SqlQueryForDataTatable(string strSql, params SqlParameter[] parameters)
        {
            return idal.SqlQueryForDataTatable(strSql, parameters);
        }

    }
}
