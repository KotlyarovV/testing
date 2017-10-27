using System;
using System.CodeDom;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;


namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void NumberValidator_CorrectInitialization()
        {
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        }
        
        [TestCase(-1, 2, true, TestName = "precision_less_than_zero")]
        [TestCase(1, 2, true, TestName = "precision_less_than_scale")]
        [TestCase(2, 2, true, TestName = "precision_equals_scale")]
        public void NumberValidator_WrongConstructorArgs_ThrowsException(int precision, int scale, bool onlyPositive)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [TestCase(3, 2, true, "-1.0", TestName = "negative_number_onlyPositive_on")]
        [TestCase(3, 2, true, "10000.0", TestName = "precision_less_than_length")]
        [TestCase(3, 2, true, "0.000", TestName = "scale_less_than_length_of_fraction")]
        public void IsValidNumber_NumbersInappropriateValidatorConditions_ShouldBeFalse
            (int precision, int scale, bool onlyPositive, string numberString)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberString).Should().BeFalse();
        }

        [TestCase(17, 2, true, "0.0", TestName = "zeros_with_dot")]
        [TestCase(17, 2, true, "2", TestName = "simple_positive_number")]
        [TestCase(3, 2, false, "-3.0", TestName = "negative_number")]
        [TestCase(4, 2, true, "+1.23", TestName = "double_number_validation")]
        public void IsValidNumber_CorrectNumberAndArgs_ShouldBeTrue(int precision, int scale, bool onlyPositive,
            string numberString)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberString).Should().BeTrue();
        }

        [TestCase("10000...0", TestName = "many_dots")]
        [TestCase(" 0  000", TestName = "whitespaces_between_numbers")]
        [TestCase("10.0.00.0", TestName = "dots_in_different_places")]
        [TestCase("a.sd", TestName = "letters_with_dot")]
        [TestCase("asd", TestName = "validator_test_wrong_format_letters")]
        public void IsValidNumber_NotNumberInput_ShouldBeFalse(string numberString)
        {
            new NumberValidator(17, 5).IsValidNumber(numberString).Should().BeFalse();
        }
    }

    public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
		    this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
            // Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
            // описанным в Формате описи документов, направляемых в налоговый орган в электронном виде 
            //по телекоммуникационным каналам связи:
            // Формат числового значения указывается в виде N(m.к), где m – максимальное количество 
            //знаков в числе, включая знак (для отрицательного числа), 
            // целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число 
            //знаков дробной части числа. 
            // Если число знаков дробной части числа равно 0 (т.е. число целое), 
            //то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}