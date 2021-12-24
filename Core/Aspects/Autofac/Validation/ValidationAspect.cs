using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Interceptors;
using Core.Utilities.Messages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect:MethodInterception
    {
        private readonly Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType)) //validatorType eğer IValidator türünde değilse..
            {
                throw new Exception(AspectMessages.WrongValidationType);
            }
            _validatorType = validatorType;
        }
        protected override void OnBefore(IInvocation invocation)
        {   //invocation kısmı bu attribute'un çağırıldığı metodun body kısmı oluyor,
            // yani kod kısmı.
            var validator = (IValidator)Activator.CreateInstance(_validatorType); //Reflection yöntemiyle instance üretme.
            var entityType = _validatorType.BaseType.GetGenericArguments()[0]; //ProductValidator içindeki implemente edilen tipe ulaşmaya çalışıyoruz.
            var entities = invocation.Arguments.Where(x => x.GetType() == entityType); //ValidationAspect'i attribute olarak çağırdığımız Metodun parametrelerine bak ve filtrele.
            foreach (var entity in entities)
            {
                ValidationTool.Validate(validator, entity);
            }
        }
    }
}
