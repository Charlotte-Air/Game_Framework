using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
// 这里须要注意几个地方： 

//1、EditorApplication.update，这个是一个delegate，能够绑定一个函数，从而在编辑器下运行Update。

//2、EditorCoroutineRunner.StartEditorCoroutine(Routine1());  这样能够在编辑器下开启一个协程。

//3、另外一个思路是不使用协程，绑定一个Update函数，然后推断www.isDone来获取AssetBundle。

//这个我并没有实际验证。

//4、www能够正常的载入出AssetBundle。可是isDone的变量一直为false。额外要注意由于Editor模式下不存在退出游戏清理资源的概念，所以要注意处理已载入的assetbundle的情况，否则可能会报冲突的错误。


//5、理论上仅仅支持yield return null这种情况，延时要自己处理。

//Unity协程的原理是引擎在特定条件下运行MoveNext运行以下的语句。在上面的代码中无论是延时还是其它的东西，都是每帧运行MoveNext，这样WaitForSeconds这种协程是无效的。

// www的情况比較特殊，尽管理论上也是会有问题的。可是确实能够正常的取到结果。
namespace SKY 
{

    public static class EditorCoroutineRunner 
    {
        private class EditorCoroutine : IEnumerator 
        {
            private Stack<IEnumerator> executionStack;

            public EditorCoroutine(IEnumerator iterator) 
            {
                this.executionStack = new Stack<IEnumerator>();
                this.executionStack.Push(iterator);
            }

            public bool MoveNext() 
            {
                IEnumerator i = this.executionStack.Peek();

                if (i.MoveNext()) 
                {
                    object result = i.Current;
                    if (result != null && result is IEnumerator) 
                    {
                        this.executionStack.Push((IEnumerator)result);
                    }
                    return true;
                } 
                else 
                {
                    if (this.executionStack.Count > 1) 
                    {
                        this.executionStack.Pop();
                        return true;
                    }
                }
                return false;
            }

            public void Reset() 
            {
                throw new System.NotSupportedException("This Operation Is Not Supported.");
            }

            public object Current 
            {
                get { return this.executionStack.Peek().Current; }
            }

            public bool Find(IEnumerator iterator) 
            {
                return this.executionStack.Contains(iterator);
            }
        }

        private static List<EditorCoroutine> editorCoroutineList;
        private static List<IEnumerator> buffer;

        public static IEnumerator StartEditorCoroutine(IEnumerator iterator) 
        {
            if (editorCoroutineList == null) 
            {
                // test
                editorCoroutineList = new List<EditorCoroutine>();
            }
            if (buffer == null) 
            {
                buffer = new List<IEnumerator>();
            }
            if (editorCoroutineList.Count == 0) 
            {
                EditorApplication.update += Update;
            }
            // add iterator to buffer first
            buffer.Add(iterator);
            return iterator;
        }

        private static bool Find(IEnumerator iterator) 
        {
            // If this iterator is already added
            // Then ignore it this time
            for (int i = 0; i < editorCoroutineList.Count; i++) 
            {
                if (editorCoroutineList[i].Find(iterator)) {
                    return true;
                }
            }
            return false;
        }

        private static void Update() 
        {
            // EditorCoroutine execution may append new iterators to buffer
            // Therefore we should run EditorCoroutine first
            editorCoroutineList.RemoveAll
            (
                coroutine => { return coroutine.MoveNext() == false; }
            );
            // If we have iterators in buffer
            if (buffer.Count > 0) {
                for (int i = 0; i < buffer.Count; i++) 
                {
                    var iterator = buffer[i];
                    // If this iterators not exists
                    if (!Find(iterator)) 
                    {
                        // Added this as new EditorCoroutine
                        editorCoroutineList.Add(new EditorCoroutine(iterator));
                    }
                }
                // Clear buffer
                buffer.Clear();
            }
            // If we have no running EditorCoroutine
            // Stop calling update anymore
            if (editorCoroutineList.Count == 0) 
            {
                EditorApplication.update -= Update;
            }
        }
    }
}
