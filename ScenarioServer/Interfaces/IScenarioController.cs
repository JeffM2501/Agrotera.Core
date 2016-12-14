using System.Collections.Generic;
using ScenarioServer.Classes;

namespace ScenarioServer.Interfaces
{
    public interface IScenarioController
    {
        string Name { get; }
        bool Defaultable { get; }

        void Init(ScenarioState state);
        void Shutdown();

        void Update(double delta);

        UserShip AddPlayerShip(int playerID, List<string> requestParams);
    }
}
