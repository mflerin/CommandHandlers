using System;
using System.Collections.Generic;

//ref: http://goodenoughsoftware.net/2013/11/
//ref:http://www.infoq.com/presentations/8-lines-code-refactoring
// Greg Young's article how to use partial application like DI.
namespace CommandHandlers
{
    class Program
    {
        static void Main(string[] args)
        {
            var dispatcher = new Dispatcher<ICommand>();

            new CommandHandlers().BootStrap(dispatcher);

            dispatcher.Dispatch(new DeactivateCommand { ProductId = 999, Reason = "A need this deactivated" });
            dispatcher.Dispatch(new ReactiveCommand { Id = 212121, ReativateReason = "I'm turning it back on" });

            Console.ReadKey();
        }
    }

    class CommandHandlers
    {
        public void Deactivate(int someInt, DeactivateCommand message)
        {
            Console.WriteLine("Deactivate");
            Console.WriteLine("Fixed parameter using partial application");
            Console.WriteLine(someInt);
            Console.WriteLine("Command Params");
            Console.WriteLine(message.ProductId);
            Console.WriteLine(message.Reason);
        }

        public void Reactivate(string someUsername, ReactiveCommand message)
        {
            Console.WriteLine("Reactivate");
            Console.WriteLine("Fixed parameter using partial application");
            Console.WriteLine(someUsername);
            Console.WriteLine("Command Params");
            Console.WriteLine(message.Id);
            Console.WriteLine(message.ReativateReason);
        }

        public static int Add(int a, int b) { return a + b; }

        /// <summary>
        /// Command Handlers all registered in one place.
        /// </summary>
        public void BootStrap(Dispatcher<ICommand> dispatcher)
        {
            //The first params are just int and strings but in the 
            //real world they'd be dependencies such as repositories
            //Uses the functional programming principle of partial application
            dispatcher.Register<DeactivateCommand>(message => Deactivate(123, message));
            dispatcher.Register<ReactiveCommand>(message => Reactivate("Flerin", message));

            //A simpler look at partial application
            //It's basically fixing the first param so the next time we call the function 
            //we only need the second param.
            var add5 = new Func<int, int>(x => Add(5, x));

            Console.WriteLine(add5(15));
        }
    }
    public interface IMessage { }
    public interface ICommand : IMessage { }
    public interface IEvent : IMessage { }

    public class DeactivateCommand : ICommand
    {
        public int ProductId { get; set; }
        public string Reason { get; set; }
    }

    public class ReactiveCommand : ICommand
    {
        public int Id { get; set; }
        public string ReativateReason { get; set; }
    }

    public class Dispatcher<TMessage>
    {
        private readonly Dictionary<Type, Action<TMessage>> _dictionary = new Dictionary<Type, Action<TMessage>>();

        public void Register<T>(Action<T> action) where T : TMessage
        {
            _dictionary.Add(typeof(T), x => action((T)x));
        }

        public void Dispatch(TMessage m)
        {
            Action<TMessage> handler;
            if (!_dictionary.TryGetValue(m.GetType(), out handler))
            {
                throw new Exception("cannot map " + m.GetType());
            }
            handler(m);
        }
    }
}
