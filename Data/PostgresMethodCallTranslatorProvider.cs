using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EgitimSitesi.Data
{
    public class PostgresMethodCallTranslatorProvider : NpgsqlMethodCallTranslatorProvider
    {
        public PostgresMethodCallTranslatorProvider(
            RelationalMethodCallTranslatorProviderDependencies dependencies)
            : base(dependencies)
        {
            var sqlExpressionFactory = dependencies.SqlExpressionFactory;
            
            // Add custom translators
            AddTranslators(new IMethodCallTranslator[]
            {
                new CustomDateTimeFunctionsTranslator(sqlExpressionFactory)
            });
        }

        // Helper method to add translators
        private void AddTranslators(IEnumerable<IMethodCallTranslator> translators)
        {
            // Get the private field _methodCallTranslators from the NpgsqlMethodCallTranslatorProvider base class
            var field = typeof(NpgsqlMethodCallTranslatorProvider)
                .GetField("_methodCallTranslators", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                var existingTranslators = (IList<IMethodCallTranslator>)field.GetValue(this);
                foreach (var translator in translators)
                {
                    existingTranslators.Add(translator);
                }
            }
        }
    }

    // Custom translator for DateTime functions
    public class CustomDateTimeFunctionsTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public CustomDateTimeFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            // Translate SQL Server's GETDATE() to PostgreSQL's NOW()
            if (method.Name == "GetDate" && method.DeclaringType == typeof(DateTime))
            {
                return _sqlExpressionFactory.Function(
                    "NOW",
                    Array.Empty<SqlExpression>(),
                    nullable: true,
                    argumentsPropagateNullability: Array.Empty<bool>(),
                    typeof(DateTime));
            }

            return null;
        }
    }
} 