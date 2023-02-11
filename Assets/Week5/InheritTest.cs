using UnityEngine;

namespace Week5
{
    public class InheritTest : MonoBehaviour
    {
        void Start()
        {
            Animal animal = new Animal();
            animal.name = "포유류";

            animal.Say();
            // animal.DoSomething();
            
            Dog dog = new Dog();
            dog.name = "푸들";
            
            dog.Say();
            dog.Bark();
        }
    }
    
    class Animal
    {
        public string name = "None";
        protected int age;

        public void Say()
        {
            Debug.Log("내이름은 " + name + ", 나이는 " + age);
        }

        protected void DoSomething()
        {
            Debug.Log("나는 동물의 원형이다");
        }
    }

    class Dog : Animal
    {
        public void Bark()
        {
            Debug.Log("멍멍");
        }
    }
}