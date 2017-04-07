using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEngine.UI;

public class Submit_Bet_Test {

    [Test]
    public void allInTest()
    {
        //Arrange
        // DOCs
        FakeServer server = new FakeServer();
        FakeSlider slider = new FakeSlider();
        FakeDisplayMessage messages = new FakeDisplayMessage();
        FakeDisableButtons buttons = new FakeDisableButtons();
        // get a new set of global vars
        FakeGlobalVars globals = new FakeGlobalVars();
        globals.player_id = "1";
        globals.game_id = "2";
        globals.chips = 2000;
        globals.bet = globals.chips;

        // set up SUT
        var test_fixture = new GameObject();
        test_fixture.AddComponent<SubmitBet_Tester>();
        SubmitBet_Tester.setUp(server, slider, globals, messages, buttons, false);

        //Act
        //Try to sent a bet
        SubmitBet_Tester.allIn();

        //Assert
        // correct message is displayed
        Assert.AreEqual("You have bet 2000.", messages.messages[0]);
        // slider is reset and deactivated
        Assert.AreEqual(0, slider.value);
        Assert.False(slider.interactable);
        // correct url sent to the server
        Assert.AreEqual("http://104.131.99.193/game/2/1/2000", server.url);
        // buttons are deactivated
        Assert.False(buttons.interactable);
        // current bet reset
        Assert.AreEqual(0, globals.curr_bet);
    }

    [Test]
    public void callTest()
    {
        //Arrange
        // DOCs
        FakeServer server = new FakeServer();
        FakeSlider slider = new FakeSlider();
        FakeDisplayMessage messages = new FakeDisplayMessage();
        FakeDisableButtons buttons = new FakeDisableButtons();
        // get a new set of global vars
        FakeGlobalVars globals = new FakeGlobalVars();
        globals.player_id = "1";
        globals.game_id = "2";
        globals.curr_bet = 500;
        globals.bet = globals.curr_bet;

        // set up SUT
        var test_fixture = new GameObject();
        test_fixture.AddComponent<SubmitBet_Tester>();
        SubmitBet_Tester.setUp(server, slider, globals, messages, buttons, false);

        //Act
        //Try to send a call
        SubmitBet_Tester.call();

        //Assert
        // correct message is displayed
        Assert.AreEqual("You have bet 500.", messages.messages[0]);
        // slider is reset and deactivated
        Assert.AreEqual(0, slider.value);
        Assert.False(slider.interactable);
        // correct url sent to the server
        Assert.AreEqual("http://104.131.99.193/game/2/1/500", server.url);
        // buttons are deactivated
        Assert.False(buttons.interactable);
        // current bet reset
        Assert.AreEqual(0, globals.curr_bet);
    }
    [Test]
    public void foldTest()
    {
        //Arrange
        // DOCs
        FakeServer server = new FakeServer();
        FakeSlider slider = new FakeSlider();
        FakeDisplayMessage messages = new FakeDisplayMessage();
        FakeDisableButtons buttons = new FakeDisableButtons();
        // get a new set of global vars
        FakeGlobalVars globals = new FakeGlobalVars();
        globals.player_id = "1";
        globals.game_id = "2";

        // set up SUT
        var test_fixture = new GameObject();
        test_fixture.AddComponent<SubmitBet_Tester>();
        SubmitBet_Tester.setUp(server, slider, globals, messages, buttons, true);

        //Act
        //Try to sent a bet
        SubmitBet_Tester.fold();

        //Assert
        // correct message is displayed
        Assert.AreEqual("You have folded.", messages.messages[0]);
        // slider is reset and deactivated
        Assert.AreEqual(0, slider.value);
        Assert.False(slider.interactable);
        // correct url sent to the server
        Assert.AreEqual("http://104.131.99.193/game/2/1/0", server.url);
        // buttons are deactivated
        Assert.False(buttons.interactable);
        // current bet reset
        Assert.AreEqual(0, globals.curr_bet);
    }

    [Test]
	public void betTest() {
        //Arrange
        // DOCs
        FakeServer server = new FakeServer();
        FakeSlider slider = new FakeSlider();
        FakeDisplayMessage messages = new FakeDisplayMessage();
        FakeDisableButtons buttons = new FakeDisableButtons();
        // get a new set of global vars
        FakeGlobalVars globals = new FakeGlobalVars();
        globals.bet = 1000;// send a bet of 1000
        globals.player_id = "1";
        globals.game_id = "2";

        // set up SUT
        var test_fixture = new GameObject();
        test_fixture.AddComponent<SubmitBet_Tester>();
        SubmitBet_Tester.setUp(server, slider, globals, messages, buttons, false);

        //Act
        //Try to sent a bet
        SubmitBet_Tester.sendTheBet();

        //Assert
        // correct message is displayed
        Assert.AreEqual("You have bet 1000.", messages.messages[0]);
        // slider is reset and deactivated
        Assert.AreEqual(0, slider.value);
        Assert.False(slider.interactable);
        // correct url sent to the server
        Assert.AreEqual("http://104.131.99.193/game/2/1/1000", server.url);
        // buttons are deactivated
        Assert.False(buttons.interactable);
        // current bet reset
        Assert.AreEqual(0, globals.curr_bet);

        
	}
}
