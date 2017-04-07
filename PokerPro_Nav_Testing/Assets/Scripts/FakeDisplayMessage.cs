using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDisplayMessage {

    public string[] messages = new string[16];
    public int numMessages = 0;

    public void print_message(string message)
    {
        messages[numMessages] = message;
        numMessages++;
    }
}
