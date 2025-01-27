using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gacfox.Wealthome.Models;

/// <summary>
/// 用户
/// </summary>
public class User
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 登录用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 展示的用户名字
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 头像链接
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 角色编码: ADMIN 管理员, MEMBER 普通用户
    /// </summary>
    public string RoleCode { get; set; }

    /// <summary>
    /// 密码哈希值
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// 用户状态: 0 用户删除状态, 1 用户激活状态, 2 用户禁用状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 管理员
    /// </summary>
    public const string RoleCodeAdmin = "ADMIN";

    /// <summary>
    /// 普通用户
    /// </summary>
    public const string RoleCodeMember = "MEMBER";

    /// <summary>
    /// 用户删除状态
    /// </summary>
    public const int UserStatusDeleted = 0;

    /// <summary>
    /// 用户激活状态
    /// </summary>
    public const int UserStatusActive = 1;

    /// <summary>
    /// 用户禁用状态
    /// </summary>
    public const int UserStatusSuspended = 2;
}

public class UserDto
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string RoleCode { get; set; }

    public string RoleName
    {
        get
        {
            return RoleCode switch
            {
                User.RoleCodeAdmin => "管理员",
                User.RoleCodeMember => "普通用户",
                _ => ""
            };
        }
    }

    [JsonIgnore] public string PasswordHash { get; set; }
    public int Status { get; set; }

    public string StatusName
    {
        get
        {
            return Status switch
            {
                User.UserStatusActive => "正常",
                User.UserStatusDeleted => "已删除",
                User.UserStatusSuspended => "已禁用",
                _ => ""
            };
        }
    }

    public DateTime CreatedAt { get; set; }

    public string CreatedAtStr => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
}

public class QueryUserReq
{
    [Required(ErrorMessage = "删除标志查询条件不能为空")]
    public bool QueryDeleted { get; set; } = false;
}

public class InitAdminReq
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(20, ErrorMessage = "用户名不能超过20个字符")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "密码不能为空")] public string Password { get; set; }
}

public class CreateUserReq
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(20, ErrorMessage = "用户名不能超过20个字符")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "用户名只能包含字母和数字")]
    public string UserName { get; set; }

    [MaxLength(20, ErrorMessage = "姓名不能超过20个字符")]
    public string? DisplayName { get; set; }

    [MaxLength(50, ErrorMessage = "邮箱不能超过50个字符")]
    public string? Email { get; set; }

    [MaxLength(255, ErrorMessage = "头像链接不能超过255个字符")]
    public string? AvatarUrl { get; set; }

    [Required(ErrorMessage = "角色编码不能为空")] public string RoleCode { get; set; }
    [Required(ErrorMessage = "密码不能为空")] public string Password { get; set; }
}

public class UpdateUserAvatarReq
{
    [Required(ErrorMessage = "文件不能为空")] public IFormFile File { get; set; }
}

public class UpdateUserStatusReq
{
    [Required(ErrorMessage = "用户主键不能为空")] public long Id { get; set; }
}

public class UpdateUserInfoReq
{
    [Required(ErrorMessage = "主键不能为空")] public long Id { get; set; }

    [MaxLength(20, ErrorMessage = "用户名不能超过20个字符")]
    public string? DisplayName { get; set; }

    [MaxLength(50, ErrorMessage = "用户名不能超过20个字符")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "角色编码")] public string RoleCode { get; set; }
}

public class LoginReq
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(20, ErrorMessage = "用户名不能超过20个字符")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "密码不能为空")] public string Password { get; set; }
}