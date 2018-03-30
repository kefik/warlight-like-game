namespace Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Pool whose purpose is to relieve the pressure on GC.
    /// </summary>
    /// <typeparam name="TType">Type parameter that must be class (otherwise it doesn't make sense)
    /// and must have parameterless construtor.
    /// </typeparam>
    /// <remarks>
    /// Support for readonly types is limited due to inability
    /// to reset its value.
    /// </remarks>
    public class ObjectPool<TType> where TType : class, new()
    {
        /// <summary>
        /// Invokes constructor for type <see cref="TType"/>
        /// on existing object.
        /// </summary>
        private static readonly Action<TType> constructorInvoker;

        /// <summary>
        /// Invokes method resetting all non-read-only and non-const
        /// fields and non-read-only auto-implemented properties.
        /// </summary>
        private static readonly Action<TType> resetInvoker;

        /// <summary>
        /// Free resources to be used by the pool.
        /// </summary>
        private readonly ConcurrentQueue<TType> freeResourcesQueue;

        static ObjectPool()
        {
            // create constructor for object of given type
            var constructor =
                typeof(TType).GetConstructor(
                    Type.EmptyTypes);
            var constructorDynamicMethod = new DynamicMethod(string.Empty,
                typeof(void), new[] {typeof(TType)},
                typeof(TType).Module, true);
            var ilGenerator = constructorDynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, constructor);
            ilGenerator.Emit(OpCodes.Ret);

            var constructorAction = (Action<TType>)constructorDynamicMethod.CreateDelegate(typeof(Action<TType>));
            constructorInvoker = constructorAction;

            // create field resetter
            var parameterExpression =
                Expression.Parameter(typeof(TType));
            var assignExpressionsList = new List<Expression>();
            // non-readonly or const fields
            var fields =
                typeof(TType).GetFields(BindingFlags.Instance |
                                        BindingFlags.Public |
                                        BindingFlags.NonPublic)
                                        .Where(x => !x.IsInitOnly && !x.IsLiteral);
            foreach (var field in fields)
            {
                var defaultExpression =
                    Expression.Default(field.FieldType);
                var fieldExpression =
                    Expression.Field(parameterExpression, field);

                var assignExpression =
                    Expression.Assign(fieldExpression,
                        defaultExpression);
                assignExpressionsList.Add(assignExpression);
            }

            // properties
            var properties =
                typeof(TType).GetProperties(BindingFlags.Instance |
                                        BindingFlags.Public |
                                        BindingFlags.NonPublic)
                                        // only those that have setters and getters both
                                        .Where(x => x.CanRead && x.CanWrite);
            foreach (var property in properties)
            {
                var defaultExpression =
                    Expression.Default(property.PropertyType);
                var fieldExpression =
                    Expression.Property(parameterExpression, property);

                var assignExpression =
                    Expression.Assign(fieldExpression,
                        defaultExpression);
                assignExpressionsList.Add(assignExpression);
            }
            // append empty expression (to be action)
            assignExpressionsList.Add(Expression.Empty());
            var blockExpression =
                Expression.Block(assignExpressionsList);
            // create lambda
            resetInvoker = Expression.Lambda<Action<TType>>(blockExpression,
                parameterExpression).Compile();
        }

        public ObjectPool()
        {
            freeResourcesQueue = new ConcurrentQueue<TType>();
        }

        /// <summary>
        /// Represents default pool for given type.
        /// </summary>
        public static ObjectPool<TType> DefaultPool { get; } = new ObjectPool<TType>();

        /// <summary>
        /// Allocates an object of <see cref="TType"/> type and returns it.
        /// </summary>
        /// <returns></returns>
        public TType Allocate()
        {
            if (freeResourcesQueue.TryDequeue(out var result))
            {
                constructorInvoker(result);
                return result;
            }

            return new TType();
        }

        /// <summary>
        /// Frees the object of <see cref="TType"/> type and adds it to the free resources in order to be used again.
        /// </summary>
        /// <param name="obj"></param>
        public void Free(TType obj)
        {
            resetInvoker.Invoke(obj);
            freeResourcesQueue.Enqueue(obj);
        }
    }
}