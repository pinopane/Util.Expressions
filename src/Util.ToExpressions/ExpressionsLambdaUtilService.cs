using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Util.ToExpression
{
    public class ExpressionsLambdaUtilService<TEntity> : IExpressionsLambdaUtilService<TEntity> where TEntity : class
    {
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        
        public Expression<Func<TEntity, bool>> CreateToExpression(string search, string searchIn)
        {
            if (string.IsNullOrEmpty(search)) { throw new ArgumentException("Param search is null."); }

            var item = Expression.Parameter(typeof(TEntity), "item");
            var booleanMatch = Regex.Match(search, @"()\s*(true|false)", RegexOptions.IgnoreCase);
            var numberMatch = Regex.Match(search, @"()\s*(number)", RegexOptions.IgnoreCase);
            var arrayMatch = Regex.Match(searchIn, @"[\&\-\[']", RegexOptions.IgnoreCase);

            searchIn = $"root.{searchIn}";
            switch (search)
            {
                case string _search when booleanMatch.Success && arrayMatch.Success:
                    var listBool = new List<bool> { _search.Equals("true") };
                    return LambdaContainsInArrayExpression(False<TEntity>(), searchIn, listBool);
                case string _search when numberMatch.Success && arrayMatch.Success:
                    var replace = _search.Replace("number", "");
                    var intSearch = Convert.ToInt32(replace);
                    var listInt = new List<int> { intSearch };
                    return LambdaContainsInArrayExpression(False<TEntity>(), searchIn, listInt);
                case string _search when booleanMatch.Success:
                    var boolSearch = _search.Equals("true");
                    return LambdaEqualsExpression(boolSearch, searchIn, item);
                case string _search when arrayMatch.Success:
                    var listarray = new List<string> { _search };
                    return LambdaContainsInArrayExpression(False<TEntity>(), searchIn, listarray);
                case string _search when numberMatch.Success:
                    var replacenumber = search.Replace("number", "");
                    var _intSearch = Convert.ToInt32(replacenumber);
                    return LambdaEqualsExpression(_intSearch, searchIn, item);
                default:
                    return LambdaEqualsExpression(search, searchIn, item);
            }
        }

        private static Expression<Func<TEntity, bool>> LambdaEqualsExpression<T>(T search, string searchIn, ParameterExpression item)
        {
            var typeParameter = TypeParameter<T>();
            var fieldParamBySearch = Expression.Constant(search);
            var containsMethodBySearch = Expression.Equal(Expression.Parameter(typeParameter, searchIn), fieldParamBySearch);
            var result = Expression.Lambda<Func<TEntity, bool>>(containsMethodBySearch, item);
            return result;
        }

        private static Expression<Func<TEntity, bool>> LambdaContainsExpression<T, TValue>(
            Expression<Func<T, bool>> predicate, List<string> filterIn, List<TValue> values)
        {
            var param = predicate.Parameters.Single();
            var property = filterIn.Aggregate<string, Expression>(param, (current, pName) => Expression.PropertyOrField(current, pName));
            var contain = typeof(List<TValue>).GetMethod("Contains");
            var mc = Expression.Call(Expression.Constant(values), contain, property);
            return Expression.Lambda<Func<TEntity, bool>>(mc, param);
        }

        private static Expression<Func<TEntity, bool>> LambdaContainsInArrayExpression<T, TValue>(Expression<Func<T, bool>> predicate, string filterIn, List<TValue> values)
        {
            var typeParameter = TypeParameter<TValue>();
            var param = predicate.Parameters.Single();
            var property = Expression.Parameter(typeParameter, filterIn);
            var contain = typeof(List<TValue>).GetMethod("Contains");
            var mc = Expression.Call(Expression.Constant(values), contain, property);
            return Expression.Lambda<Func<TEntity, bool>>(mc, param);
        }

        private static Type TypeParameter<T>()
        {
            Type typeParameter = null;
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.String:
                    typeParameter = typeof(string);
                    break;
                case TypeCode.Boolean:
                    typeParameter = typeof(bool);
                    break;
                case TypeCode.Int32:
                    typeParameter = typeof(int);
                    break;
                default:
                    typeParameter = typeof(string);
                    break;
            }
            return typeParameter;
        }
    }
}