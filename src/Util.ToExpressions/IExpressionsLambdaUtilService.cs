using System;
using System.Linq.Expressions;

namespace Util.ToExpression
{
    public interface IExpressionsLambdaUtilService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Turn the search parameter and search fields into a lambda expression. For numbers search params use the word "number", example: "number1"
        /// </summary>
        /// <param name="search"></param>
        /// <param name="searchIn"></param>
        /// <returns>Lambda Expression.</returns>
        Expression<Func<TEntity, bool>> CreateToExpression(string search, string searchIn);
    }
}