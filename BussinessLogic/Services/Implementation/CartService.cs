﻿using AutoMapper;
using DataAccess.Data;
using DataAccess.Entities;
using BusinessLogic.Constants;
using BusinessLogic.DTOs.Cart;
using BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.DTOs.Payment;
using System.Net.Http;

namespace BusinessLogic.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;
        private readonly IMapper _mapper;
        
        public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CartDto?> GetCartAsync(string userName)
        {
            _logger.LogInformation("Retrieving cart for user: {UserName}", userName);

            var userRepository = _unitOfWork.GetRepository<User>();
            var user = await userRepository.FindAsync(u => u.Username == userName);
            var userEntity = user.FirstOrDefault();

            if (userEntity == null)
            {
                _logger.LogWarning("User {UserName} not found.", userName);
                throw new ArgumentException("User not found.");
            }

            var cartRepository = _unitOfWork.GetRepository<Cart>();
            var cart = await cartRepository.FindAsync(c => c.UserId == userEntity.UserId && c.Status == CartStatusConstants.Active);
            var activeCart = cart.FirstOrDefault();

            if (activeCart == null)
            {
                _logger.LogInformation("No active cart found for user: {UserName}", userName);
                return null; // Controller will handle creating an empty car
            }

            // Load cart items with products
            var cartItemRepository = _unitOfWork.GetRepository<CartItem>();
            var cartItems = await cartItemRepository.FindAsync(ci => ci.CartId == activeCart.CartId, includeProperties: "Product.Lab");

            var cartItemDtos = cartItems.Select(ci => new CartItemDto
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product.ProductName,
                Quantity = ci.Quantity,
                Price = ci.Price,
                TotalPrice = ci.Price * ci.Quantity,
                LabFileUrl = ci.Product.Lab?.LabFileUrl
            }).ToList();

            var cartDto = new CartDto
            {
                CartId = activeCart.CartId,
                UserId = activeCart.UserId,
                CreatedDate = activeCart.CreatedDate,
                Status = activeCart.Status,
                Items = cartItemDtos,
                TotalAmount = cartItemDtos.Sum(ci => ci.TotalPrice)
            };

            _logger.LogInformation("Cart retrieved successfully for user: {UserName}", userName);
            return cartDto;
        }

        public async Task<string> AddItemToCartAsync(string userName, int productId, int quantity)
        {
            _logger.LogInformation("Adding ProductId: {ProductId} to cart for User: {UserName}", productId, userName);

            var userRepository = _unitOfWork.GetRepository<User>();
            var user = await userRepository.FindAsync(u => u.Username == userName);
            var userEntity = user.FirstOrDefault();

            if (userEntity == null)
            {
                _logger.LogWarning("User {UserName} not found.", userName);
                throw new ArgumentException("User not found.");
            }

            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found.", productId);
                throw new ArgumentException("Product not found.");
            }

            if (product.StockQuantity < quantity)
            {
                _logger.LogWarning("Insufficient stock for ProductId: {ProductId}. Requested: {Quantity}, Available: {Stock}", productId, quantity, product.StockQuantity);
                throw new ArgumentException("Insufficient stock for the requested product.");
            }

            // Retrieve or create active cart
            var cartRepository = _unitOfWork.GetRepository<Cart>();
            var cart = await cartRepository.FindAsync(c => c.UserId == userEntity.UserId && c.Status == CartStatusConstants.Active);
            var activeCart = cart.FirstOrDefault();

            if (activeCart == null)
            {
                activeCart = new Cart
                {
                    UserId = userEntity.UserId,
                    CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = CartStatusConstants.Active
                };
                await cartRepository.AddAsync(activeCart);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Created new cart for UserId: {UserId}", userEntity.UserId);
            }

            // Check if product already in cart
            var cartItemRepository = _unitOfWork.GetRepository<CartItem>();
            var existingCartItem = await cartItemRepository.FindAsync(ci => ci.CartId == activeCart.CartId && ci.ProductId == productId);
            var cartItem = existingCartItem.FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.Price = product.Price; // Update price in case it has changed
                cartItemRepository.Update(cartItem);
                _logger.LogInformation("Updated quantity for CartItemID: {CartItemId}", cartItem.CartItemId);
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = activeCart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                await cartItemRepository.AddAsync(cartItem);
                _logger.LogInformation("Added new CartItemID: {CartItemId} to CartID: {CartId}", cartItem.CartItemId, activeCart.CartId);
            }

            // Deduct stock
            product.StockQuantity -= quantity;
            productRepository.Update(product);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Item added to cart successfully for UserId: {UserId}", userEntity.UserId);
            return "Item added to cart successfully.";
        }

        public async Task<string> UpdateCartItemAsync(string userName, int cartItemId, int quantity)
        {
            _logger.LogInformation("Updating CartItemID: {CartItemId} for User: {UserName} to Quantity: {Quantity}", cartItemId, userName, quantity);

            // Step 1: Retrieve the user
            var userRepository = _unitOfWork.GetRepository<User>();
            var user = await userRepository.FindAsync(u => u.Username == userName);
            var userEntity = user.FirstOrDefault();

            if (userEntity == null)
            {
                _logger.LogWarning("User {UserName} not found.", userName);
                throw new ArgumentException("User not found.");
            }

            // Step 2: Retrieve the CartItem with the related Cart entity
            var cartItemRepository = _unitOfWork.GetRepository<CartItem>();
            var cartItem = await cartItemRepository.GetAsync(
                ci => ci.CartItemId == cartItemId,
                includeProperties: "Cart"
            );

            if (cartItem == null)
            {
                _logger.LogWarning("CartItemID: {CartItemId} not found or does not belong to an active cart.", cartItemId);
                throw new ArgumentException("Cart item not found.");
            }

            if (cartItem.Cart.UserId != userEntity.UserId)
            {
                _logger.LogWarning("CartItemID: {CartItemId} does not belong to user: {UserName}.", cartItemId, userName);
                throw new ArgumentException("Cart item does not belong to the current user.");
            }

            if (cartItem.Cart.Status != CartStatusConstants.Active)
            {
                _logger.LogWarning("CartItemID: {CartItemId} is not part of an active cart.", cartItemId);
                throw new ArgumentException("Cannot update items in a non-active cart.");
            }

            // Step 3: Retrieve the Product
            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetByIdAsync(cartItem.ProductId);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for CartItemID: {CartItemId}.", cartItem.ProductId, cartItemId);
                throw new ArgumentException("Product not found.");
            }

            // Step 4: Update stock based on quantity change
            if (quantity > cartItem.Quantity)
            {
                int additionalQuantity = quantity - cartItem.Quantity;
                if (product.StockQuantity < additionalQuantity)
                {
                    _logger.LogWarning("Insufficient stock for ProductId: {ProductId}. Requested Additional: {AdditionalQuantity}, Available: {Stock}", product.ProductId, additionalQuantity, product.StockQuantity);
                    throw new ArgumentException("Insufficient stock for the requested quantity.");
                }
                product.StockQuantity -= additionalQuantity;
            }
            else if (quantity < cartItem.Quantity)
            {
                int reducedQuantity = cartItem.Quantity - quantity;
                product.StockQuantity += reducedQuantity;
            }

            // Step 5: Update CartItem details
            cartItem.Quantity = quantity;
            cartItem.Price = product.Price; // Update price if it has changed
            cartItemRepository.Update(cartItem);
            productRepository.Update(product);

            // Step 6: Save changes
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("CartItemID: {CartItemId} updated successfully.", cartItemId);
            return "Cart item updated successfully.";
        }

        public async Task<string> RemoveItemFromCartAsync(string userName, int cartItemId)
        {
            _logger.LogInformation("Removing CartItemID: {CartItemId} from cart for User: {UserName}", cartItemId, userName);

            // Step 1: Retrieve the user
            var userRepository = _unitOfWork.GetRepository<User>();
            var user = await userRepository.FindAsync(u => u.Username == userName);
            var userEntity = user.FirstOrDefault();

            if (userEntity == null)
            {
                _logger.LogWarning("User {UserName} not found.", userName);
                throw new ArgumentException("User not found.");
            }

            // Step 2: Retrieve the CartItem with the related Cart entity
            var cartItemRepository = _unitOfWork.GetRepository<CartItem>();
            var cartItem = await cartItemRepository.GetAsync(
                ci => ci.CartItemId == cartItemId,
                includeProperties: "Cart"
            );

            if (cartItem == null)
            {
                _logger.LogWarning("CartItemID: {CartItemId} not found or does not belong to an active cart.", cartItemId);
                throw new ArgumentException("Cart item not found.");
            }

            if (cartItem.Cart.UserId != userEntity.UserId)
            {
                _logger.LogWarning("CartItemID: {CartItemId} does not belong to user: {UserName}.", cartItemId, userName);
                throw new ArgumentException("Cart item does not belong to the current user.");
            }

            if (cartItem.Cart.Status != CartStatusConstants.Active)
            {
                _logger.LogWarning("CartItemID: {CartItemId} is not part of an active cart.", cartItemId);
                throw new ArgumentException("Cannot update items in a non-active cart.");
            }

            var productRepository = _unitOfWork.GetRepository<Product>();
            var product = await productRepository.GetByIdAsync(cartItem.ProductId);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for CartItemID: {CartItemId}.", cartItem.ProductId, cartItemId);
                throw new ArgumentException("Product not found.");
            }

            // Restore stock
            product.StockQuantity += cartItem.Quantity;
            productRepository.Update(product);

            // Remove cart item
            cartItemRepository.Delete(cartItem);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("CartItemID: {CartItemId} removed successfully.", cartItemId);
            return "Item removed from cart successfully.";
        }

        public async Task<string> ClearCartAsync(string userName)
        {
            _logger.LogInformation("Clearing cart for User: {UserName}", userName);

            var userRepository = _unitOfWork.GetRepository<User>();
            var user = await userRepository.FindAsync(u => u.Username == userName);
            var userEntity = user.FirstOrDefault();

            if (userEntity == null)
            {
                _logger.LogWarning("User {UserName} not found.", userName);
                throw new ArgumentException("User not found.");
            }

            var cartRepository = _unitOfWork.GetRepository<Cart>();
            var cart = await cartRepository.FindAsync(c => c.UserId == userEntity.UserId && c.Status == CartStatusConstants.Active);
            var activeCart = cart.FirstOrDefault();

            if (activeCart == null)
            {
                _logger.LogInformation("No active cart found for User: {UserName}", userName);
                return "No active cart to clear.";
            }

            var cartItemRepository = _unitOfWork.GetRepository<CartItem>();
            var cartItems = await cartItemRepository.FindAsync(ci => ci.CartId == activeCart.CartId);

            foreach (var cartItem in cartItems)
            {
                var productRepository = _unitOfWork.GetRepository<Product>();
                var product = await productRepository.GetByIdAsync(cartItem.ProductId);

                if (product != null)
                {
                    product.StockQuantity += cartItem.Quantity;
                    productRepository.Update(product);
                }

                cartItemRepository.Delete(cartItem);
            }

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Cart cleared successfully for User: {UserName}", userName);
            return "Cart cleared successfully.";
        }

        public async Task<PaymentResult> CheckoutAsync(string userName, CheckoutDto checkoutDto)
        {
            _logger.LogInformation("Checkout initiated for User: {UserName}", userName);

            try
            {
                // Validate và lấy thông tin giỏ hàng
                var cartDetails = await ValidateAndGetCartDetails(userName);

                // Tạo yêu cầu thanh toán qua PayOS
                var paymentService = new PaymentService(_httpClient, _logger);
                var paymentResult = await paymentService.CreatePaymentRequestAsync(
                    cartDetails.TotalAmount,
                    $"ORDER_{cartDetails.Cart.CartId}_{DateTime.UtcNow.Ticks}",
                    $"Payment for cart {cartDetails.Cart.CartId}"
                );

                if (paymentResult.Success)
                {
                    // Cập nhật trạng thái giỏ hàng (optional)
                    cartDetails.Cart.Status = CartStatusConstants.PendingPayment;
                    _unitOfWork.GetRepository<Cart>().Update(cartDetails.Cart);
                    await _unitOfWork.CompleteAsync();
                }

                return paymentResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed for User: {UserName}", userName);
                throw;
            }
        }

        // Thêm constant mới
        public static class CartStatusConstants
        {
            public const string Active = "Active";
            public const string PendingPayment = "PendingPayment";
            public const string CheckedOut = "CheckedOut";
        }
    }
}