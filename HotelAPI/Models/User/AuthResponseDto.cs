namespace HotelAPI.Models.User
{
    //https://developers.zalo.me/docs/api/social-api/tham-khao/co-che-het-han-cua-user-refresh-token-post-6749#:~:text=User%20Refresh%20Token%20l%C3%A0%20th%C3%B4ng,Access%20Token%20%C4%91%C3%A3%20h%E1%BA%BFt%20h%E1%BA%A1n.&text=Khi%20b%E1%BA%A1n%20d%C3%B9ng%201%20Refresh,c%E1%BB%A7a%20Refresh%20Token%20truy%E1%BB%81n%20v%C3%A0o.
    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        /// <summary>
        /// Refresh Token là thông tin cần để lấy User Access Token mới trong trường hợp User Access Token đã hết hạn.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
