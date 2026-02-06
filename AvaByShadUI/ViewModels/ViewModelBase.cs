using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AvaByShadUI.ViewModels;

public abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo, IDisposable
{
    private readonly Dictionary<string, List<string>> _errors = [];
    private bool _disposed;

    public bool HasErrors => _errors.Count != 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return Array.Empty<string>();
        }

        return _errors.TryGetValue(propertyName, out var errors)
            ? errors
            : Array.Empty<string>();
    }

    protected void SetProperty<T>(ref T field, T value, bool validate = false,
        [CallerMemberName] string propertyName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;

        field = value;
        OnPropertyChanged(propertyName);
        if (validate) ValidateProperty(value, propertyName);
    }

    protected void ValidateProperty<T>(T value, string propertyName)
    {
        ClearErrors(propertyName);

        var validationContext = new ValidationContext(this)
        {
            MemberName = propertyName
        };
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateProperty(value, validationContext, validationResults)) return;

        foreach (var validationResult in validationResults)
        {
            AddError(propertyName, validationResult.ErrorMessage ?? string.Empty);
        }
    }

    protected void AddError(string propertyName, string error)
    {
        if (!_errors.TryGetValue(propertyName, out List<string>? value))
        {
            value = [];
            _errors[propertyName] = value;
        }

        if (value.Contains(error)) return;
        value.Add(error);
        OnErrorsChanged(propertyName);
    }

    protected void ClearErrors(string propertyName)
    {
        if (_errors.Remove(propertyName)) OnErrorsChanged(propertyName);
    }

    protected void ClearAllErrors()
    {
        var properties = _errors.Keys.ToList();
        _errors.Clear();
        foreach (var property in properties) OnErrorsChanged(property);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ValidateAllProperties()
    {
        var properties = GetType().GetProperties()
            .Where(prop => prop.GetCustomAttributes(typeof(ValidationAttribute), true).Length != 0);

        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            ValidateProperty(value, property.Name);
        }
    }


    #region 信使

    /// <summary>
    /// 信使实例，用于发送和接收消息
    /// </summary>
    protected IMessenger Messenger { get; }

    /// <summary>
    /// 构造函数，使用默认的 WeakReferenceMessenger
    /// </summary>
    protected ViewModelBase() : this(WeakReferenceMessenger.Default)
    {
    }

    /// <summary>
    /// 构造函数，允许注入自定义的 Messenger 实例
    /// </summary>
    /// <param name="messenger">信使实例</param>
    protected ViewModelBase(IMessenger messenger)
    {
        Messenger = messenger;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="message">消息实例</param>
    protected void Send<TMessage>(TMessage message) where TMessage : class
    {
        Messenger.Send(message);
    }

    /// <summary>
    /// 发送消息到指定频道
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="TToken">频道令牌类型</typeparam>
    /// <param name="message">消息实例</param>
    /// <param name="token">频道令牌</param>
    protected void Send<TMessage, TToken>(TMessage message, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Messenger.Send(message, token);
    }

    /// <summary>
    /// 注册消息接收器
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="handler">消息处理器</param>
    protected void Register<TMessage>(MessageHandler<object, TMessage> handler)
        where TMessage : class
    {
        Messenger.Register(this, handler);
    }

    /// <summary>
    /// 注册指定频道的消息接收器
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="TToken">频道令牌类型</typeparam>
    /// <param name="token">频道令牌</param>
    /// <param name="handler">消息处理器</param>
    protected void Register<TMessage, TToken>(TToken token, MessageHandler<object, TMessage> handler)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Messenger.Register(this, token, handler);
    }

    /// <summary>
    /// 取消注册所有消息接收器
    /// </summary>
    protected void UnregisterAll()
    {
        Messenger.UnregisterAll(this);
    }

    /// <summary>
    /// 取消注册指定类型的消息接收器
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    protected void Unregister<TMessage>() where TMessage : class
    {
        Messenger.Unregister<TMessage>(this);
    }

    /// <summary>
    /// 取消注册指定频道的消息接收器
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="TToken">频道令牌类型</typeparam>
    /// <param name="token">频道令牌</param>
    protected void Unregister<TMessage, TToken>(TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken>
    {
        Messenger.Unregister<TMessage, TToken>(this, token);
    }

    #endregion

    /// <summary>
    ///     Disposes the ViewModel and cleans up resources. Override to add custom cleanup logic.
    /// </summary>
    public virtual void Dispose()
    {
        if (_disposed) return;

        // Clear errors and event handlers
        ClearAllErrors();
        ErrorsChanged = null;

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~ViewModelBase()
    {
        Dispose();
    }
}