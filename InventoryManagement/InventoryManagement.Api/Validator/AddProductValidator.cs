using FluentValidation;
using InventoryManagement.Data.Entity;
using InventoryManagement.Data.Repository;
using InventoryManagement.Model.EntityModels;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagement.Api.Validator
{
    /// <summary>
    /// 
    /// </summary>
    public class AddProductValidator : AbstractValidator<AddProduct>
    {
        // Manually assigning the Subcategories for simple Validation, else Fetch them from DB. Since I am using In Memory DB for the CRUD Operations mentioning these data Manually.
        readonly List<string> subCategories = new List<string>()
        {
            "Shoes","Slippers","sandals","Heels","Sneakers","RunningShoes","Gym Shoes","TrekkingShoes","Formals","FlipFlops","Flats"
        };

        public AddProductValidator()
        {
            RuleFor(a => a.ProductName).NotEmpty().WithMessage("ProductName is missing").Must(a => a.Length > 6).WithMessage("Product Name should be minimum of 6 Characters");
            RuleFor(a => a.ProductDescription).NotEmpty().WithMessage("Description is missing").Must(a => a.Length < 120).WithMessage("Product Description should not be more than 120 Characters");
            RuleFor(a => a.Brand).NotEmpty().WithMessage("Brand is missing").Matches(@"^[a-zA-Z][a-zA-Z0-9]*$").WithMessage("InValid BrandName");
            RuleFor(a => a.Category).NotEmpty().WithMessage("Category is missing")
                .Custom((a, context) =>
                {
                    if (a.ToLower() != "apparel")
                    {
                        context.AddFailure(context.DisplayName, "InValid Category");
                    }

                });
            RuleFor(a => a.SubCategory).NotEmpty().WithMessage("Sub Category is Missing").Custom((a, context) =>
            {
                if (!subCategories.Contains(a))
                {
                    context.AddFailure(context.DisplayName, $"Sub Category should be in {string.Join(',', subCategories)}");
                }
            });
        }
    }
}
