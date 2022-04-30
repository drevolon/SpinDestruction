public class InteractiveData
{
    public Car CurrentCar { get; }

    public SubscriptionProperty<GameState> gameState { get; set; } = new SubscriptionProperty<GameState>();
    public InteractiveData(Car car)
    {
        CurrentCar = car;
    }

    
}