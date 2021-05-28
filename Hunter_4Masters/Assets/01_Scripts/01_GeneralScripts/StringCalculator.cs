using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class StringCalculator
{
    private struct oper
    {
        public int p;  // 연산자 우선순위
        public string o; // 연산자

        public oper(int p, string o)
        {
            this.p = p;
            this.o = o;
        }
    }

    static Stack<float> num = new Stack<float>(); // 숫자 스택
    static Stack<oper> op = new Stack<oper>(); // 연산자 스택

    private static void calc()
    {
        float a, b, res = 0;
        b = num.Peek();
        num.Pop();
        a = num.Peek();
        num.Pop();
        string oper = op.Peek().o;
        op.Pop();

        if (oper == "*") res = a * b;
        else if (oper == "/") res = a / b;
        else if (oper == "+") res = a + b;
        else if (oper == "-") res = a - b;

        num.Push(res);
    }

    public static float Calculate(string str)
    {
        string[] tok = str.Split(new char[] { ' ' });    // 공백을 기준으로 잘라 배열에 넣음
        float number;
        for (int i = 0; i < tok.Length; i++)
        {
            // ( 는 무조건 연산자 스택에 push
            if (tok[i] == "(")
            {
                op.Push(new oper(0, tok[i]));
            }   // ) 가 나오면 ( 가 나올때까지 계산
            else if (tok[i] == ")")
            {
                while (op.Peek().o != "(") calc();
                op.Pop();
            }
            else if (tok[i] == "*" || tok[i] == "/" || tok[i] == "+" || tok[i] == "-")
            {
                int prior = 0;  // 연산자 우선순위
                if (tok[i] == "*") prior = 2;
                else if (tok[i] == "/") prior = 2;
                else if (tok[i] == "+") prior = 1;
                else if (tok[i] == "-") prior = 1;

                // 연산자 우선순위 낮은게 top으로 올때까지 계산
                while (op.Count != 0 && prior <= op.Peek().p) calc();
                // 스택에 연산자 push
                op.Push(new oper(prior, tok[i]));
            }
            else if(Single.TryParse(tok[i], out number)){ // 숫자일 경우 숫자 스택에 push
                num.Push(number);
            }
            else // str, dex와 같은 문자열은 육성데이터에 대입하여 숫자로 변환 후 숫자 스택에 push
            {
                PlayerData pd = FosterManager.Instance.GetPlayerData();
                num.Push(pd.GetCurPointOfAllType(tok[i]));
            }
        }
        while (op.Count != 0) calc();

        return num.Peek();
    }
}
