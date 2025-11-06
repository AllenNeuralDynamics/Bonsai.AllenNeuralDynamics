using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Bonsai;
using Bonsai.Expressions;

namespace AllenNeuralDynamics.Core
{
    [Description("Checks if an object contains a specific key.")]
    public class ContainsKeyBuilder : BinaryOperatorBuilder
    {
        protected override Expression BuildSelector(Expression expression)
        {
            Expression left, right;
            var operand = Operand;

            var expressionTypeDefinition = expression.Type.IsGenericType ? expression.Type.GetGenericTypeDefinition() : null;
            if (expressionTypeDefinition == typeof(Tuple<,>))
            {
                Operand = null;
                left = ExpressionHelper.MemberAccess(expression, "Item1");
                right = ExpressionHelper.MemberAccess(expression, "Item2");
                return BuildSelector(left, right);
            }
            else
            {
                var indexerProperties = expression.Type.GetDefaultMembers()
                    .OfType<PropertyInfo>()
                    .Where(property => property.GetIndexParameters().Length == 1)
                    .Select(property => property.GetIndexParameters())
                    .ToArray();
                var indexerTypes = indexerProperties;
                if (indexerTypes.Length == 0)
                {
                    throw new InvalidOperationException("The specified expression does not have an indexer property.");
                }

                var operandType = indexerTypes[0][0].ParameterType;
                if (operand == null)
                {
                    if (operandType != typeof(string) &&
                       (operandType.IsInterface ||
                        operandType.IsClass && operandType.GetConstructor(Type.EmptyTypes) == null))
                    {
                        throw new InvalidOperationException("Must provide a valid constructor for the specified key type.");
                    }

                    var propertyType = GetWorkflowPropertyType(operandType);
                    Operand = operand = (WorkflowProperty)Activator.CreateInstance(propertyType);
                }

                left = expression;
                var operandExpression = Expression.Constant(operand);
                right = Property(operandExpression, "Value");
            }

            return BuildSelector(left, right);
        }

        private static MemberExpression Property(Expression expression, string propertyName)
        {
            var type = expression.Type;
            while (type != null)
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
                var property = type.GetProperty(propertyName, bindingFlags | BindingFlags.Public);
                if (property == null)
                    property = type.GetProperty(propertyName, bindingFlags | BindingFlags.NonPublic);
                if (property != null)
                    return Expression.Property(expression, property);

                type = type.BaseType;
            }

            throw new ArgumentException(string.Format("Instance property '{0}' is not defined for type '{1}'", propertyName, expression.Type));
        }

        protected override Expression BuildSelector(Expression left, Expression right)
        {
            MethodInfo containsKeyMethod = left.Type.GetMethod("ContainsKey", new Type[] { right.Type });

            if (containsKeyMethod != null)
            {
                return Expression.Call(left, containsKeyMethod, right);
            }

            MethodInfo containsMethod = left.Type.GetMethod("Contains", new Type[] { right.Type });

            if (containsMethod != null)
            {
                return Expression.Call(left, containsMethod, right);
            }

            throw new System.Exception(string.Format("Type {0} does not have a ContainsKey or Contains method that accepts {1}", left.Type.Name, right.Type.Name));
        }

        private static Type GetWorkflowPropertyType(Type expressionType)
        {
            if (expressionType == typeof(bool)) return typeof(BooleanProperty);
            if (expressionType == typeof(int)) return typeof(IntProperty);
            if (expressionType == typeof(float)) return typeof(FloatProperty);
            if (expressionType == typeof(double)) return typeof(DoubleProperty);
            if (expressionType == typeof(string)) return typeof(StringProperty);
            if (expressionType == typeof(DateTime)) return typeof(DateTimeProperty);
            if (expressionType == typeof(TimeSpan)) return typeof(TimeSpanProperty);
            if (expressionType == typeof(DateTimeOffset)) return typeof(DateTimeOffsetProperty);
            return typeof(WorkflowProperty<>).MakeGenericType(expressionType);
        }
    }


}
