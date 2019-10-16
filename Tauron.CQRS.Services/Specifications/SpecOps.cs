using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.CQRS.Services.Specifications
{
    [PublicAPI]
    public static class SpecOps
    {
        private sealed class SimpleSpec<TType> : SpecificationBase<TType>
        {
            private readonly Func<TType, bool> _eval;

            public SimpleSpec(Func<TType, bool> eval, string msg)
            {
                _eval = eval;
                Message = msg;
            }

            public override string Message { get; }

            protected override bool IsSatisfiedBy(TType target) 
                => _eval(target);
        }

        private sealed class AndNotSpecification : ISpecification
        {
            private readonly ISpecification _left;
            private readonly ISpecification _right;

            public AndNotSpecification(ISpecification left, ISpecification right)
            {
                _left = left;
                _right = right;
            }

            public string Message { get; private set; }

            public bool IsSatisfiedBy(object obj)
            {
                if (_left.IsSatisfiedBy(obj))
                {
                    if (_right.IsSatisfiedBy(obj) != true)
                        return true;

                    Message = $"!{_right.Message}";
                }
                else
                    Message = _left.Message;

                return false;
            }
        }

        private sealed class AndSpecification : ISpecification
        {
            private readonly ISpecification _left;
            private readonly ISpecification _right;

            public AndSpecification(ISpecification left, ISpecification right)
            {
                _left = left;
                _right = right;
            }

            public string Message { get; private set; }

            public bool IsSatisfiedBy(object obj)
            {
                if (_left.IsSatisfiedBy(obj))
                {
                    if (_right.IsSatisfiedBy(obj))
                        return true;

                    Message = _right.Message;
                }
                else
                    Message = _left.Message;

                return false;
            }
        }

        private sealed class NotSpecification : ISpecification
        {
            private readonly ISpecification _right;
            public string Message => $"!{_right.Message}";

            public NotSpecification(ISpecification right) 
                => _right = right;

            public bool IsSatisfiedBy(object left) 
                => !_right.IsSatisfiedBy(left);
        }

        private class OrNotSpecification : ISpecification
        {
            private readonly ISpecification _left;
            private readonly ISpecification _right;

            public OrNotSpecification(ISpecification left, ISpecification right)
            {
                _left = left;
                _right = right;
            }

            public string Message { get; private set; }

            public bool IsSatisfiedBy(object obj)
            {
                if (_left.IsSatisfiedBy(obj))
                    return true;
                if (_right.IsSatisfiedBy(obj) != true)
                    return true;

                Message = $"-- {_left.Message} -- !{_right.Message} --";

                return false;
            }
        }

        private class OrSpecification : ISpecification
        {
            private readonly ISpecification _left;
            private readonly ISpecification _right;

            public OrSpecification(ISpecification left, ISpecification right)
            {
                _left = left;
                _right = right;
            }

            public string Message { get; private set; }

            public bool IsSatisfiedBy(object obj)
            {
                if (_left.IsSatisfiedBy(obj) || _right.IsSatisfiedBy(obj))
                    return true;

                Message = $"-- {_left.Message} || {_right.Message}";

                return false;
            }
        }

        public static ISpecification Simple<TType>(Func<TType, bool> eval, string msg) 
            => new SimpleSpec<TType>(eval, msg);

        public static ISpecification AndNot(this ISpecification left, ISpecification right) 
            => new AndNotSpecification(left, right);

        public static ISpecification And(this  ISpecification left, ISpecification right)
            => new AndSpecification(left, right);

        public static ISpecification Not(this ISpecification left)
            => new NotSpecification(left);

        public static ISpecification OrNot(this ISpecification left, ISpecification right)
            => new OrNotSpecification(left, right);

        public static ISpecification Or(this ISpecification left, ISpecification right)
            => new OrSpecification(left, right);
    }
}