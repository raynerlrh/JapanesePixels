using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Agent
{
    public class AgentMemory
    {
        private Stack<int> correctAnswers;
        public int GetSize
        {
            get
            {
                return correctAnswers.Count;
            }
        }
        private int last_answer; // last correct answer
        private readonly int max_memory;
        public int newMaxMemory;

        public AgentMemory(int MaxMemory)
        {
            max_memory = MaxMemory;
            newMaxMemory = MaxMemory;
            correctAnswers = new Stack<int>(MaxMemory);
        }

        /// <summary>
        /// Get the last correct answer by peeping into stack
        /// </summary>
        /// <returns>Last correct answer</returns>
        public int getLastAnswer()
        {
            if (correctAnswers.Count > 0)
            {
                last_answer = correctAnswers.Peek();
                return last_answer;
            }
            return 0;
        }

        public void storeAnswer(int answer)
        {
            correctAnswers.Push(answer);
        }

        public void forgetAnswer()
        {
            for (int i = 0; i < correctAnswers.Count; ++i)
                correctAnswers.Push(correctAnswers.Pop());
            correctAnswers.Pop();

            for (int i = 0; i < correctAnswers.Count; ++i)
                correctAnswers.Push(correctAnswers.Pop());
        }

        public bool doesBrainContain(int element)
        {
            return correctAnswers.Contains(element);
        }
    }
}
