using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace JetBlack.MessageBus.Common.Collections.Json
{
    /// <summary>
    /// A simple tree class.
    /// </summary>
    /// <typeparam name="T">The type of the tree nodes.</typeparam>
    [JsonObject]
    public class Node<T> : IEnumerable<T>
        where T : Node<T>
    {
        [JsonProperty("children")]
        private readonly List<T> _children = new List<T>();

        /// <summary>
        /// Add a child to this node.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item)
        {
            _children.Add(item);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
