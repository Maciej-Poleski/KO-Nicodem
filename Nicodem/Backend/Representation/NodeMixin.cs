using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nicodem.Backend.Representation
{
    internal static class NodeMixin
    {
        private static readonly Dictionary<Type, MethodInfo> NodeTypeToHandler =
            (from methodInfo in
                typeof (AbstractVisitor).GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                    BindingFlags.DeclaredOnly)
                let parameters = methodInfo.GetParameters()
                where parameters.Length == 1 && typeof (Node).IsAssignableFrom(parameters[0].ParameterType)
                select new {key = parameters[0].ParameterType, value = methodInfo})
                .ToDictionary(arg => arg.key.IsGenericType ? arg.key.GetGenericTypeDefinition() : arg.key,
                    arg => arg.value);

        internal static Action<AbstractVisitor> MakeAcceptThunk<TNode>(TNode @this) where TNode : Node
        {
            var visitorParam = Expression.Parameter(typeof (AbstractVisitor), "visitor");
            var thisParam = Expression.Constant(@this);
            var thisType= @this.GetType();
            var cleanType = thisType.IsGenericType ? thisType.GetGenericTypeDefinition() : thisType;
            var method = NodeTypeToHandler[cleanType];
            if (thisType.IsGenericType)
            {
                method = method.MakeGenericMethod(thisType.GetGenericArguments());
            }
            var visitCallExpression = Expression.Call(visitorParam, method, thisParam);
            var function = Expression.Lambda<Action<AbstractVisitor>>(visitCallExpression, visitorParam);
            return function.Compile();
        }
    }
}