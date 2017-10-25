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
        public void TestNumberValidatorCorrectInitialization()
        {
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        }
        
        [TestCase(-1, 2, true, TestName = "precision_less_than_zero")]
        [TestCase(1, 2, true, TestName = "precision_less_than_scale")]
        [TestCase(2, 2, true, TestName = "precision_equals_scale")]
        public void TestNumberValidatorThrows(int precision, int scale, bool onlyPositive)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [TestCase(17, 2, true, "0.0", ExpectedResult = true, TestName = "0.0_check_right_validation_with_dot")]
        [TestCase(17, 2, true, "2", ExpectedResult = true, TestName = "simple_positive_number")]
        [TestCase(17, 2, false, "-3", ExpectedResult = true, TestName = "simple_negative_number_onlypositive_off")]
        [TestCase(3, 2, true, "-0.00", ExpectedResult = false, TestName = "negative_zero_little_precision")]

        [TestCase(3, 2, true, "+0.00", ExpectedResult = false, TestName = "positive_zero_little_precision")]
        [TestCase(4, 2, true, "+1.23", ExpectedResult = true, TestName = "double_number_validation")]
        [TestCase(3, 2, true, "+1.23", ExpectedResult = false, TestName = "double_number_validation_little_precision")]
        [TestCase(3, 2, true, "a.sd", ExpectedResult = false, TestName = "letters_with_dot")]
        [TestCase(3, 2, true, "asd", ExpectedResult = false, TestName = "validator_test_wrong_format_letters")]

        [TestCase(3, 2, false, "-0.0", ExpectedResult = true, TestName = "negative_zero")]
        [TestCase(3, 2, true, "+0.0", ExpectedResult = true, TestName = "positive_zero")]
        [TestCase(3, 2, true, "-1.0", ExpectedResult = false, TestName = "negative_number_onlyPositive_on")]
        [TestCase(3, 2, true, "10000.0", ExpectedResult = false, TestName = "precision_less_than_length")]

        [TestCase(9, 2, true, "10000...0", ExpectedResult = false, TestName = "many_dots")]
        [TestCase(9, 2, true, "10.0.00.0", ExpectedResult = false, TestName = "few_dots_in_different_places")]
        [TestCase(3, 2, true, "0.000", ExpectedResult = false, TestName = "scale_less_than_length")]
        [TestCase(10, 5, true, "0  000", ExpectedResult = false, TestName = "whitespaces_between_numbers")]
        public bool TestNumberValidator(int precision, int scale, bool onlyPositive, string numberString)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberString);
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