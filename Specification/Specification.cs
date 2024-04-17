using System.Linq.Expressions;

namespace motoMeet
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
    }

    public class Specification<T>
    {
        private readonly List<Expression<Func<T, bool>>> _criteria = new List<Expression<Func<T, bool>>>();
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public Specification<T> AddCriteria(Expression<Func<T, bool>> criteria)
        {
            _criteria.Add(criteria);
            return this;
        }

        public Specification<T> AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
            return this;
        }

        public Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> combinedExpression = _criteria.FirstOrDefault();

            foreach (var criteria in _criteria.Skip(1))
            {
                combinedExpression = combinedExpression.CombineWith(criteria);
            }

            return combinedExpression ?? (x => true); // Return an always true expression if no criteria
        }
    }

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> CombineWith<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
            var left = leftVisitor.Visit(first.Body);

            var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
            var right = rightVisitor.Visit(second.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }

}