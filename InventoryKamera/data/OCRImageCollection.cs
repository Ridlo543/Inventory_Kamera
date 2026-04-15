using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace InventoryKamera
{
	public class OCRImageCollection
	{
		public List<Bitmap> Bitmaps { get; set; }
		public string Type { get; set; }
		public int Id { get; private set; }

		public OCRImageCollection(List<Bitmap> _bm, string _type, int _id)
		{
			Bitmaps = _bm;
			Type = _type;
			Id = _id;
		}
	}

	public class Queue<T>
	{
		/// <summary>Used as a lock target to ensure thread safety.</summary>
		private readonly object _Locker = new object();
		private readonly AutoResetEvent _HasItems = new AutoResetEvent(false);

		private readonly System.Collections.Generic.Queue<T> _Queue = new System.Collections.Generic.Queue<T>();

		/// <summary></summary>
		public void Enqueue(T item)
		{
			lock (_Locker)
			{
				_Queue.Enqueue(item);
			}
			_HasItems.Set();
		}

		/// <summary>Enqueues a collection of items into this queue.</summary>
		public virtual void EnqueueRange(IEnumerable<T> items)
		{
			bool anyAdded = false;
			lock (_Locker)
			{
				if (items == null)
				{
					return;
				}

				foreach (T item in items)
				{
					_Queue.Enqueue(item);
					anyAdded = true;
				}
			}

			if (anyAdded)
			{
				_HasItems.Set();
			}
		}

		/// <summary></summary>
		public T Dequeue()
		{
			lock (_Locker)
			{
				return _Queue.Dequeue();
			}
		}

		/// <summary></summary>
		public void Clear()
		{
			lock (_Locker)
			{
				_Queue.Clear();
			}
		}

		/// <summary></summary>
		public Int32 Count
		{
			get
			{
				lock (_Locker)
				{
					return _Queue.Count;
				}
			}
		}

		/// <summary></summary>
		public Boolean TryDequeue(out T item)
		{
			lock (_Locker)
			{
				if (_Queue.Count > 0)
				{
					item = _Queue.Dequeue();
					if (_Queue.Count > 0)
					{
						_HasItems.Set();
					}
					return true;
				}
				else
				{
					item = default(T);
					return false;
				}
			}
		}

		public bool WaitForItem(int timeoutMs)
		{
			lock (_Locker)
			{
				if (_Queue.Count > 0)
				{
					return true;
				}
			}

			return _HasItems.WaitOne(timeoutMs);
		}

		public void Signal()
		{
			_HasItems.Set();
		}
	}
}