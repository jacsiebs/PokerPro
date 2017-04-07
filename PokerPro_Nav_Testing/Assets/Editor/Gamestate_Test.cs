using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEngine.UI;

public class Gamestate_Test {



	[Test]
	public void GamestateTest() {
        //Arrange
        // DOCs
        FakeServer server = new FakeServer();
        FakeGlobalVars globals = new FakeGlobalVars();
        globals.player_id = "20";
        globals.game_id = "1";

        // set up SUT
        var test_fixture = new GameObject();
        test_fixture.AddComponent<FakeDebugChangeCard>();

        // parse the test game state
        //Assert
        //check if pot is as expected
        Assert.AreEqual(FakeDebugChangeCard.JSONGameState(globals.game_id, globals.player_id, "pot"), "0");

        //check if num players is as expected
        Assert.AreEqual(FakeDebugChangeCard.JSONGameState(globals.game_id, globals.player_id, "players"), "2");
    }
}
