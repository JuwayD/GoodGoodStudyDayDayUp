namespace Script.DesignerModel
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
    
    /// <summary>
    /// 简单工厂
    /// </summary>
    public static class CarFactory
    {
        public static Car CreateCar(string carType)
        {
            switch (carType)
            {
                case "BMW":
                    return new BMW();
                case "Audi":
                    return new Audi();
                case "Honda":
                    return new Honda();
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// 工厂方法模式
    /// </summary>
    public abstract class CarFactoryFunc
    {
        public abstract Car CreateCar();
    }
    
    public class BMWFactory : CarFactoryFunc
    {
        public override Car CreateCar()
        {
            return new BMW();
        }
    }
    
    public class AudiFactory : CarFactoryFunc
    {
        public override Car CreateCar()
        {
            return new Audi();
        }
    }
    
    public class HondaFactory : CarFactoryFunc
    {
        public override Car CreateCar()
        {
            return new Honda();
        }
    }
    
    /// <summary>
    /// 抽象工厂模式
    /// </summary>
    public abstract class AbstractFactory
    {
        public abstract Audi CreateCarFactory();
        public abstract BMW CreateBMWFactory();
    }
    
    public class XiaoMiFactory : AbstractFactory
    {
        public override Audi CreateCarFactory()
        {
            return new Audi();
        }

        public override BMW CreateBMWFactory()
        {
            return new BMW();
        }
    }

    }

    public class Honda : Car
    {
    }

    public class Audi : Car
    {
    }

    public class BMW : Car
    {
    }

    public class Car
    {
    }