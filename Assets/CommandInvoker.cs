using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    static Queue<ICommand> commandBuffer;
    static List<ICommand> commandHistory;
    static int counter;

    void Awake()
    {
        commandBuffer = new Queue<ICommand>();
        commandHistory = new List<ICommand>();
    }

    public void AddCommand(ICommand command)
    {
        while(commandHistory.Count > counter)
        {
            commandHistory.RemoveAt(counter);
        }

        commandBuffer.Enqueue(command);
    }

    // Update is called once per frame
    void Update()
    {
        if(commandBuffer.Count > 0)
        {
            ICommand c = commandBuffer.Dequeue();
            c.Execute();

            commandHistory.Add(c);
            counter++;
        }
        else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
        {
            if(counter > 0)
            {
                counter--;
                commandHistory[counter].PerformUndo();
            }
        }
        else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Y))
        {
            if(counter < commandHistory.Count)
            {
                print("yes");
                commandHistory[counter].Execute();
                counter++;
            }
        }
    }
}
