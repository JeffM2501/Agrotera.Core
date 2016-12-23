using System.Collections.Generic;
using Entities.Classes;

namespace ScenarioServer.Interfaces
{
    public interface IScenarioController
    {
        string Name { get; }
        bool Defaultable { get; }

        void Init(ScenarioState state);
        void Shutdown();

        void Update(double delta);

        Ship AddPlayerShip(long playerID, List<string> requestParams);
    }
}
