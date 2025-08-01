namespace MediatorData.Contracts;

public interface IRequest { }
public interface IRequest<out TResponse> { }