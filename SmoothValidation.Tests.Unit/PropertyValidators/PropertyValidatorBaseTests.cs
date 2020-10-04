using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.PropertyValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using SmoothValidation.Types;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.Tests.Unit.PropertyValidators
{
    [TestFixture]
    public class PropertyValidatorBaseTests
    {
        [Test]
        public void For_Ctor_When_ProvidedMemberInfoValueIsNull_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => new PropertyValidatorBaseTestImpl<string>(null));
        }

        [Test]
        public void For_Ctor_When_ProvidedMemberInfoValueIsValid_Then_ValidatorIsInitialized()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();

            // Act:
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);

            // Assert:
            validator.Member.IsProperty.Should().BeTrue();
            validator.Member.Name.Should().Be("SomeProperty");
            validator.PropertyDisplayName.Should().Be("SomeProperty");
        }

        [Test]
        public void For_AddRule_When_PredicateIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            Predicate<string> predicate = null;

            // Act:
            Assert.Throws<ArgumentNullException>(() => validator.AddRule(predicate, It.IsAny<string>(), null));
        }

        [Test]
        public void For_AddRule_When_ErrorMessageIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            Predicate<string> anyPredicate = s => s != null;

            // Act:
            Assert.Throws<ArgumentNullException>(() => validator.AddRule(anyPredicate, null, null));
        }

        [Test]
        public void For_AddRule_When_ProvidedArgumentsAreValid_Then_ValidationTaskIsAdded()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            Predicate<string> anyPredicate = s => s != null;
            const string errorMessage = "SOME ERROR MESSAGE";

            // Act:
            validator.AddRule(anyPredicate, errorMessage, null);

            // Assert:
            validator.GetValidationTasksTestMethod().Count.Should().Be(1);
            var validationTask = validator.GetValidationTasksTestMethod().First();
            validationTask.IsOtherValidator.Should().Be(false);
            validationTask.StopValidationAfterFailure.Should().Be(false);
            validationTask.Validator.Should().NotBeNull();
        }

        [Test]
        public void For_SetValidator_When_OtherValidatorIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            
            // Act:
            Assert.Throws<ArgumentNullException>(() => validator.SetValidator(null));
        }

        [Test]
        public void For_SetValidator_When_PassedValidatorReferenceConcernsTargetValidator_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            
            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() => validator.SetValidator(validator));
            exception.Message.Should().Contain("Detected circular reference");
        }

        [Test]
        public void For_SetValidator_When_OtherValidatorIsAlreadySet_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            var validationTask = new ValidationTask(Mock.Of<IValidator>(), true);
            validator.AddValidationTasksTestMethod(new List<ValidationTask> {validationTask});
            
            // Act:
            var exception = Assert.Throws<InvalidOperationException>(() => 
                validator.SetValidator(Mock.Of<ISyncValidator<string>>()));
            exception.Message.Should().Contain("There is already set other validator");
        }

        [Test]
        public void For_SetValidator_When_ValidatorCanBeSet_Then_ItIsAddedToValidationTasksList()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            
            // Act:
            validator.SetValidator(Mock.Of<ISyncValidator<string>>());
            
            // Assert:
            var validationTasks = validator.GetValidationTasksTestMethod();
            validationTasks.Count.Should().Be(1);
        }

        [Test]
        public void For_StopValidationAfterFailure_When_ThereAreNoValidationTasks_Then_ConstraintIsNotAppliedToAnyValidationTask()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);

            // Act:
            validator.StopValidationAfterFailure();

            // Assert:
            var validationTasks = validator.GetValidationTasksTestMethod();
            validationTasks.Count.Should().Be(0);
        }

        [Test]
        public void For_StopValidationAfterFailure_When_ThereAre2ValidationTasks_Then_ConstraintIsAppliedToLastValidationTask()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            var validationTasks = new List<ValidationTask>
            {
                new ValidationTask(Mock.Of<IValidator>(), false),
                new ValidationTask(Mock.Of<IValidator>(), false)
            };
            validator.AddValidationTasksTestMethod(validationTasks);

            // Act:
            validator.StopValidationAfterFailure();

            // Assert:
            var resultValidationTasks = validator.GetValidationTasksTestMethod();
            resultValidationTasks.Count.Should().Be(2);
            resultValidationTasks.First().StopValidationAfterFailure.Should().BeFalse();
            resultValidationTasks.Skip(1).First().StopValidationAfterFailure.Should().BeTrue();
        }

        [Test]
        public void For_WithMessage_When_ProvidedMessageIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);

            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => validator.WithMessage(null));
        }

        [Test]
        public void For_WithMessage_When_ThereAreNoValidationTasks_Then_MessageIsNotOverridenForAnyValidationTask()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            const string message = "SOME MESSAGE";

            // Act:
            validator.WithMessage(message);

            // Assert:
            var validationTasks = validator.GetValidationTasksTestMethod();
            validationTasks.Count.Should().Be(0);
        }

        [Test]
        public void For_WithMessage_When_ThereAre2ValidationTasks_Then_MessageIsChangedInLastValidationTask()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            var validationTasks = new List<ValidationTask>
            {
                new ValidationTask(Mock.Of<IValidator>(), false),
                new ValidationTask(Mock.Of<IValidator>(), false)
            };
            validator.AddValidationTasksTestMethod(validationTasks);
            const string message = "SOME MESSAGE";

            // Act:
            validator.WithMessage(message);

            // Assert:
            var resultValidationTasks = validator.GetValidationTasksTestMethod();
            resultValidationTasks.Count.Should().Be(2);
            resultValidationTasks.First().ErrorTransformation.OverridenMessage.Should().BeNull();
            resultValidationTasks.Skip(1).First().ErrorTransformation
                .OverridenMessage.Should().Be(message);
        }

        [TestCase(null)]
        [TestCase("\t\t")]
        [TestCase("  ")]
        [TestCase("@Bool")]
        [TestCase("9Type")]
        [TestCase("_Parent.@static")]
        public void For_SetPropertyDisplayName_When_ProvidedDisplayNameIsInvalid_Then_ExceptionIsThrown(string propertyDisplayName)
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            var previousDisplayName = validator.PropertyDisplayName;

            // Act & Assert:
            Assert.Throws(Is.AssignableTo<Exception>(), () => validator.SetPropertyDisplayName(propertyDisplayName));
            previousDisplayName.Should().Be("SomeProperty");
        }

        [Test]
        public void For_SetPropertyDisplayName_When_ProvidedDisplayNameIsValid_Then_ItIsSet()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            const string propertyDisplayName = "SOME_PROPERTY_NAME";
            var previousDisplayName = validator.PropertyDisplayName;

            // Act:
            validator.SetPropertyDisplayName(propertyDisplayName);
            
            // Assert:
            previousDisplayName.Should().Be("SomeProperty");
            validator.PropertyDisplayName.Should().Be(propertyDisplayName);
        }

        [Test]
        public void For_WithCode_When_ProvidedCodeIsNull_Then_ExceptionIsNotThrown()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);

            // Act & Assert:
            Assert.DoesNotThrow(() => validator.WithCode(null));
        }

        [Test]
        public void For_WithCode_When_ThereAreNoValidationTasks_Then_MessageIsNotOverridenForAnyValidationTask()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            const string code = "SOME CODE";

            // Act:
            validator.WithCode(code);

            // Assert:
            var validationTasks = validator.GetValidationTasksTestMethod();
            validationTasks.Count.Should().Be(0);
        }

        [Test]
        public void For_WithCode_When_ThereAre2ValidationTasks_Then_CodeIsChangedInLastValidationTask()
        {
            // Arrange:
            var memberInfo = CreateTestMemberInfo();
            var validator = new PropertyValidatorBaseTestImpl<string>(memberInfo);
            var validationTasks = new List<ValidationTask>
            {
                new ValidationTask(Mock.Of<IValidator>(), false),
                new ValidationTask(Mock.Of<IValidator>(), false)
            };
            validator.AddValidationTasksTestMethod(validationTasks);
            const string code = "SOME CODE";

            // Act:
            validator.WithCode(code);

            // Assert:
            var resultValidationTasks = validator.GetValidationTasksTestMethod();
            resultValidationTasks.Count.Should().Be(2);
            resultValidationTasks.First().ErrorTransformation.OverriddenCode.Should().BeNull();
            resultValidationTasks.Skip(1).First().ErrorTransformation
                .OverriddenCode.Should().Be(code);
        }

        private static MemberInfo CreateTestMemberInfo()
        {
            Expression<Func<TestClass, string>> expression = obj => obj.SomeProperty;
            var memberExpression = expression.Body as MemberExpression;
            return typeof(TestClass).GetProperty(memberExpression.Member.Name);
        }

        private class PropertyValidatorBaseTestImpl<TProp> : 
            PropertyValidatorBase<PropertyValidatorBaseTestImpl<TProp>, object, string>, 
            ISyncValidator<string>
        {
            public PropertyValidatorBaseTestImpl(MemberInfo memberInfo) : base(memberInfo)
            {
            }

            internal override PropertyValidatorBaseTestImpl<TProp> PropertyValidator => this;

            public IList<ValidationTask> GetValidationTasksTestMethod()
            {
                return ValidationTasks;
            }

            public void AddValidationTasksTestMethod(IList<ValidationTask> validationTasks)
            {
                ValidationTasks.AddRange(validationTasks);
            }

            public new string PropertyDisplayName
            {
                get => base.PropertyDisplayName;
                set => PropertyDisplayName = value;
            }

            public IList<ValidationError> Validate(string obj)
            {
                throw new NotImplementedException();
            }

            public IList<ValidationError> Validate(object obj)
            {
                throw new NotImplementedException();
            }
        }

        private class TestClass
        {
            public string SomeProperty { get; set; }
        }
    }
}
