using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace StartupApi.Infrastructure
{
    internal class SortOptionsProcessor<T, TEntity>
    {
        private readonly string[] orderBy;

        public SortOptionsProcessor(string[] orderBy)
        {
            this.orderBy = orderBy;
        }

        public IEnumerable<SortTerm> GetAllTerms()
        {
            if (orderBy == null) yield break;

            foreach (var term in orderBy)
            {
                if (string.IsNullOrEmpty(term)) continue;

                var tokens = term.Split(' ');

                if (tokens.Length == 0)
                {
                    yield return new SortTerm { Name = term };
                    continue;
                }

                var descending = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
                yield return new SortTerm
                {
                    Name = tokens[0],
                    Descenting = descending
                };
            }
        }

        public IEnumerable<SortTerm> GetValidTerms()
        {
            var queryTerms = GetAllTerms().ToArray();
            if (!queryTerms.Any()) yield break;

            var declaredTerms = GetTermsFromModel();

            foreach (var term in queryTerms)
            {
                var declaredTerm = declaredTerms
                    .SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));
                if (declaredTerm == null) continue;

                yield return new SortTerm
                {
                    Name = declaredTerm.Name,
                    Descenting = term.Descenting,
                    Default = declaredTerm.Default
                };
            }
        }

        internal IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();

            if (!terms.Any()) terms = GetTermsFromModel().Where(x => x.Default).ToArray();

            if (!terms.Any()) return query;

            var modifiedQuery = query;
            var useThenBy = false;

            foreach (var term in terms)
            {
                var propertyInfo = ExpressionHelper
                       .GetPropertyInfo<TEntity>(term.Name);

                var obj = ExpressionHelper.Parameter<TEntity>();

                var key = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);
                var keySelector = ExpressionHelper.GetLambda(typeof(TEntity), propertyInfo.PropertyType, obj, key);

                modifiedQuery = ExpressionHelper
                    .CallOrderByOrThenBy(modifiedQuery, useThenBy, term.Descenting, propertyInfo.PropertyType, keySelector);
                useThenBy = true;
            }
            return modifiedQuery;
        }



        private static IEnumerable<SortTerm> GetTermsFromModel()
            => typeof(T).GetTypeInfo()
            .DeclaredProperties
            .Where(x => x.GetCustomAttributes<SortableAttribute>().Any())
            .Select(x => new SortTerm
            {
                Name = x.Name,
                Default = x.GetCustomAttribute<SortableAttribute>().Default
            });
    }
}