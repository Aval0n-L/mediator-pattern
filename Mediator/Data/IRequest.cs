namespace Mediator.Data;

public interface IRequest { }
public interface IRequest<out TResponse> { }