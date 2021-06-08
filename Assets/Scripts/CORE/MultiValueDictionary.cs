#region ��� �ڸ�Ʈ
/// <license>
///		<copyright> A_Library (for C# & Unity) </copyright>
///		<author> ������(actdoll.2001~/ 2013~2018). </author>
///		<studio> CF��.GNSS��.POC��.ONE��.�̵��ư��߽�.����Ʈ���߽�. </studio>
///		<company> NDREAM - FunGrove / Netmarble NPark (AniPark) </company>
///		<summary> �������� ����ϼŵ� �����ϴٸ�, �� ������ ��ó ������ �����ִ� ����~ </summary>
/// </license>
#endregion// ��� �ڸ�Ʈ

using System;
using System.Collections;
using System.Collections.Generic;

//	<see cref="https://stackoverflow.com/questions/380595/multimap-in-net?answertab=active#tab-top"/>
namespace CORE
{
	/// <summary>
	/// ������ ��ųʸ�
	/// - Ű ���� �ߺ��Ǿ� �������� ������ �� �ִ� �����̳�
	/// - ������ �����̳� �� ���� �ߺ��� �� �ִ�. (��, ������ Key-Value �� ��� ���Ե� �� �ִ�.)
	/// - C++ �� multimap �� (������ Ʋ������) ������ �����̴�.
	/// </summary>
	/// <typeparam name="TKey">Ű ����</typeparam>
	/// <typeparam name="TValue">�� ����</typeparam>
	public class MultiValueDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
	{
		static readonly List<TValue> EmptyValues = new List<TValue>();
		List<TKey> tmp_keys = new List<TKey>(16);

		public MultiValueDictionary()
			: base()
		{
		}
		public MultiValueDictionary(IDictionary<TKey, List<TValue>> dictionary)
			: base(dictionary)
		{
		}
		public MultiValueDictionary(IDictionary<TKey, List<TValue>> dictionary, IEqualityComparer<TKey> keycomparer)
			: base(dictionary, keycomparer)
		{
		}
		public MultiValueDictionary(IEqualityComparer<TKey> keycomparer)
			: base(keycomparer)
		{
		}
		public MultiValueDictionary(int capacity)
			: base(capacity)
		{
		}
		public MultiValueDictionary(int capacity, IEqualityComparer<TKey> keycomparer)
			: base(capacity, keycomparer)
		{
		}


		/// <summary>
		/// ����� �� ��� �����ۿ� ���� ������ Ȯ��
		/// - Key/Value ����
		/// </summary>
		/// <example>
		/// foreach(var pair in repo.Repository)
		/// {
		///		TKey key = pair.Key;
		///		TValue value = pair.Value;
		///		...
		/// }
		/// </example>
		public RepoEnumerator.Enumerable Repository => new RepoEnumerator.Enumerable(this);


		/// <summary>
		/// ��ϵ� ������ �� ����
		/// </summary>
		public int ValueCount
		{
			get
			{
				int count = 0;
				foreach(var pair in this)
					count += pair.Value?.Count ?? 0;
				return count;
			}
		}


		/// <summary>
		/// ��� Ű�� ������ �߰�
		/// - ������ �ߺ��� ����
		/// </summary>
		public void Add(in TKey key, in TValue value)
		{
			if(!this.TryGetValue(key, out var container))
			{
				container = MakeValueContainer();
				base.Add(key, container);
			}
			else if(container == null)
			{
				container = MakeValueContainer();
				base[key] = container;
			}
			container.Add(value);
		}


		/// <summary>
		/// ��� Ű�� �����۹迭 �߰�
		/// - ���� �����۹迭�� �ߺ��Ǵ� �������� �����ϰ� �߰���
		/// </summary>
		public void AddRange(in TKey key, IEnumerable<TValue> collection)
		{
			if(!this.TryGetValue(key, out var container))
			{
				base.Add(key, MakeValueContainer(collection));
				return;
			}
			else if(container == null)
			{
				base[key] = MakeValueContainer(collection);
				return;
			}
			container.AddRange(collection);
		}


		/// <summary>
		/// ��� Ű�� �ش� �������� �����ϴ°�?
		/// </summary>
		public bool ContainsValue(in TKey key, in TValue value)
		{
			bool toReturn = false;
			if(this.TryGetValue(key, out var container) && container != null)
			{
				toReturn = container.Contains(value);
			}
			return toReturn;
		}


		/// <summary>
		/// �ش� �������� �����ϴ°�?
		/// </summary>
		public bool ContainsValue(in TValue value)
		{
			foreach(var pair in this)
			{
				if(ContainsValue(pair.Key, value))
					return true;
			}
			return false;
		}


		/// <summary>
		/// ��� Ű�� �ش� �����۵� �� ���ǿ� �����ϴ� ���� �ִ°�?
		/// </summary>
		public TValue FindValue(in TKey key, Predicate<TValue> pred)
		{
			if(pred == null)
				return default;

			if(this.TryGetValue(key, out var container) && container != null)
			{
				return container.Find(pred);
			}
			return default;
		}


		/// <summary>
		/// ��� Ű�� �ش� �������� ����
		/// - �Ȱ��� �������� ���� ��� ���� ���� �߰ߵ� ���� ������
		/// - ���� �� �������� ������ Ű �����̳� ���ŵ�
		/// </summary>
		/// <returns>ã�Ƽ� �����ٸ� true. ��ã������ false</returns>
		public bool Remove(in TKey key, in TValue value)
		{
			if(this.TryGetValue(key, out var container) && container != null)
			{
				var ret = container.Remove(value);
				if(container.Count <= 0)
				{
					this.Remove(key);
				}
				return ret;
			}
			return false;
		}


		/// <summary>
		/// ��� Ű�� �ش� ���ǿ� �´� �����۵��� ��� ����
		/// - ���� �� �������� ������ Ű �����̳� ���ŵ�
		/// </summary>
		/// <returns>���� �׸��� ����</returns>
		public int RemoveAll(in TKey key, Predicate<TValue> pred)
		{
			if(pred == null)
				return 0;

			if(this.TryGetValue(key, out var container) && container != null)
			{
				var ret = container.RemoveAll(pred);
				if(container.Count <= 0)
				{
					this.Remove(key);
				}
				return ret;
			}
			return 0;
		}


		/// <summary>
		/// ��� Ű�� �ش� ���ǿ� �´� �����۵��� ��� ����
		/// - ���� �� �������� ������ Ű �����̳� ���ŵ�
		/// </summary>
		/// <returns>���� �׸��� ����</returns>
		public int RemoveAll(Predicate<TValue> pred)
		{
			if(pred == null)
				return 0;

			var ret = 0;
			foreach(var pair in this)
			{
				ret += pair.Value.RemoveAll(pred);
				if(pair.Value.Count <= 0)
					tmp_keys.Add(pair.Key);
			}

			foreach(var e in tmp_keys)
				this.Remove(e);

			tmp_keys.Clear();

			return ret;
		}


		/// <summary>
		/// ��� �����̳��� ��� ������ ������ �� �����̳ʷ� ����
		/// - �����۵��� �ߺ��˻� ����. 
		/// - ��� �����̳ʴ� ���� �״�� ����
		/// </summary>
		public void Merge(MultiValueDictionary<TKey, TValue> toMergeWith)
		{
			if(toMergeWith == null)
			{
				return;
			}

			foreach(var pair in toMergeWith)
			{
				this.AddRange(pair.Key, pair.Value);
			}
		}


		/// <summary>
		/// �ش� Ű�� ������ ���� ����
		/// </summary>
		/// <param name="key"></param>
		public int GetValuesCount(in TKey key)
		{
			return (this.TryGetValue(key, out var container) && container != null) ? container.Count : 0;
		}


		/// <summary>
		/// ��� Ű�� ���� ������ �����̳� Ȯ��
		/// - for �� ���� ��ȯó�� �ÿ��� ������ GetValues()/GetRepository() �� ����� ���� ������.
		/// </summary>
		/// <param name="key">Ű ��</param>
		/// <param name="returnEmptySet">������ �����̳ʰ� ���� ��� true �̸� �� �����̳� �����ؼ� ����. false �� null ����</param>
		/// <returns>
		/// Ű�� ������ �����̳� �ν��Ͻ�. Ű�� ������ returnEmptySet �� ���¿� ���� �� �����̳� �Ǵ� null ����.
		/// </returns>
		public List<TValue> GetValueContainer(in TKey key, bool returnEmptySet = false)
		{
			if((!this.TryGetValue(key, out var container) || container == null) && returnEmptySet)
			{
				container = MakeValueContainer();
			}
			return container;
		}


		/// <summary>
		/// ��� Ű�� ������ �����̳ʿ� ���� ������ Ȯ��
		/// - Value ����
		/// </summary>
		/// <param name="key">Ű ��</param>
		/// <example>
		/// foreach(TValue value in repo.GetValues(key))
		/// {
		///		...
		/// }
		/// </example>
		public ValueEnumerable GetValues(in TKey key)
		{
			return (this.TryGetValue(key, out var container) && container != null) ? 
				new ValueEnumerable(container) : default;
		}


		/// <summary>
		/// ��� Ű�� �����ۿ� ���� ������ Ȯ��
		/// - Key/Value ����
		/// </summary>
		/// <example>
		/// foreach(var pair in repo.GetRepository(key))
		/// {
		///		TKey key = pair.Key;
		///		TValue value = pair.Value;
		///		...
		/// }
		/// </example>
		public RepoEnumerator.Enumerable GetRepository(in TKey key)
		{
			return (this.TryGetValue(key, out var container) && container != null) ?
				new RepoEnumerator.Enumerable(key, container) : default;
		}

		//--------------------------------------------------------------------

		//	��� �����̳� ������
		List<TValue> MakeValueContainer(IEnumerable<TValue> collection = null)
		{
			List<TValue> container;
			if(collection != null)
			{
				container = new List<TValue>(collection);
			}
			else
			{
				container = new List<TValue>(8);
			}
			return container;
		}

		//============================================================================================================================================================

		#region ������ ������

		//	���Ű��� Ŭ���� - Value
		public struct ValueEnumerable : IEnumerable<TValue>, IEnumerable
		{
			List<TValue> m_repo;

			public ValueEnumerable(List<TValue> list) => m_repo = list;

			/// <summary>
			/// �⺻ ������
			/// </summary>
			public List<TValue>.Enumerator GetEnumerator() => (m_repo ?? EmptyValues).GetEnumerator();

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
		}

		//	������ - Key/Value
		public struct RepoEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
		{
			static readonly MultiValueDictionary<TKey, TValue> EmptyRepo = new MultiValueDictionary<TKey, TValue>();

			//	���Ű��� Ŭ���� - Key/Value
			public struct Enumerable : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
			{
				MultiValueDictionary<TKey, TValue> m_repo;
				List<TValue> m_list;
				TKey m_key;

				public Enumerable(MultiValueDictionary<TKey, TValue> repo)
				{
					m_repo = repo;
					m_list = null;
					m_key = default;
				}

				public Enumerable(in TKey key, List<TValue> list)
				{
					m_repo = null;
					m_list = list ?? EmptyValues;
					m_key = key;
				}

				/// <summary>
				/// �⺻ ������
				/// </summary>
				public RepoEnumerator GetEnumerator() => m_repo != null ? 
					new RepoEnumerator(m_repo ?? EmptyRepo) :			// ��ü��ȯ
					new RepoEnumerator(m_key, m_list ?? EmptyValues);	// ������ȯ

				IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => this.GetEnumerator();
				IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
			}
			//--------------------------------------------------------------------

			Enumerator m_enumRepo;
			List<TValue>.Enumerator m_enumList;
			TKey m_key;
			bool m_range;
			//
			public KeyValuePair<TKey, TValue> Current => m_range ?
				new KeyValuePair<TKey, TValue>(m_key, m_enumList.Current) :
				new KeyValuePair<TKey, TValue>(m_enumRepo.Current.Key, m_enumList.Current);

			RepoEnumerator(Dictionary<TKey, List<TValue>> repo)
			{
				m_enumRepo = repo.GetEnumerator();
				m_enumList = EmptyValues.GetEnumerator();
				m_key = default;
				m_range = false;
			}
			RepoEnumerator(TKey key, List<TValue> list)
			{
				m_enumRepo = EmptyRepo.GetEnumerator();
				m_enumList = list.GetEnumerator();
				m_key = key;
				m_range = true;
			}

			public void Dispose()
			{
				m_enumRepo.Dispose();
				m_enumList.Dispose();
			}

			public bool MoveNext()
			{
				while(true)
				{
					if(MoveNextValue())
						return true;

					if(MoveNextKey())
						continue;

					break;
				}
				return false;
			}
			bool MoveNextValue() => m_enumList.MoveNext();
			bool MoveNextKey()
			{
				if(m_range)
					return false;

				while(m_enumRepo.MoveNext())
				{
					//	üũ�� ���� �������� ������ üũ (��� ������ ���� ���� üũ�Ѵ�.
					var list = m_enumRepo.Current.Value;
					if(list != null && list.Count > 0)
					{
						m_enumList = list.GetEnumerator();
						return true;
					}
				}
				m_enumList = EmptyValues.GetEnumerator();
				return false;   // �� �̻� �Ѿ ���� ����.
			}

			void IEnumerator.Reset()
			{
				(m_enumRepo as IEnumerator).Reset();
				if(m_range)
					(m_enumList as IEnumerator).Reset();
				else
					m_enumList = EmptyValues.GetEnumerator();
			}
			object IEnumerator.Current => this.Current;
		}
		#endregion
	}
}