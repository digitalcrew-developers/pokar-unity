using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.SimpleSlider.Scripts
{
	/// <summary>
	/// Performs center/focus on child and swipe features.
	/// </summary>
	[RequireComponent(typeof(ScrollRect))]
	public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public ScrollRect ScrollRect;
		public GameObject Pagination;
		public int SwipeThreshold = 50;
		public float SwipeTime = 0.5f;

		private Toggle[] _pageToggles;

		private bool _drag;
		private bool _lerp;
		private int _page;
		private float _dragTime;

		/// <summary>
		/// Initializes scroll rect and paginator.
		/// </summary>
		/// <param name="random"></param>
		public void Initialize(bool random = false)
		{
			ScrollRect.horizontalNormalizedPosition = 0;
			_pageToggles = Pagination.GetComponentsInChildren<Toggle>(true);
			
			if (random)
			{
				ShowRandom();
			}

			UpdatePaginator(_page);
			enabled = true;
		}

		/// <summary>
		/// Performs focusing on target page.
		/// </summary>
		public void Update()
		{
			if (!_lerp || _drag) return;

			if (Pagination)
			{
				var page = GetCurrentPage();

				if (!_pageToggles[page].isOn)
				{
					UpdatePaginator(page);
				}
			}

			var horizontalNormalizedPosition = (float) _page / (ScrollRect.content.childCount - 1);

			ScrollRect.horizontalNormalizedPosition = Mathf.Lerp(ScrollRect.horizontalNormalizedPosition, horizontalNormalizedPosition, 5 * Time.deltaTime);

			if (Math.Abs(ScrollRect.horizontalNormalizedPosition - horizontalNormalizedPosition) < 0.001f)
			{
				ScrollRect.horizontalNormalizedPosition = horizontalNormalizedPosition;
				_lerp = false;
			}
		}

		/// <summary>
		/// Show random banner (immediately).
		/// </summary>
		public void ShowRandom()
		{
			if (ScrollRect.content.childCount <= 1) return;

			int page;

			do
			{
				page = UnityEngine.Random.Range(0, ScrollRect.content.childCount);
			}
			while (page == _page);

			_lerp = false;
			_page = page;
			ScrollRect.horizontalNormalizedPosition = (float) _page / (ScrollRect.content.childCount - 1);
		}

		/// <summary>
		/// Show next page.
		/// </summary>
		public void SlideNext()
		{
			Slide(1);
		}

		/// <summary>
		/// Show prev page.
		/// </summary>
		public void SlidePrev()
		{
			Slide(-1);
		}

		private void Slide(int direction)
		{
			direction = Math.Sign(direction);

			if (_page == 0 && direction == -1 || _page == ScrollRect.content.childCount - 1 && direction == 1) return;

			_lerp = true;
			_page += direction;
		}

		private int GetCurrentPage()
		{
			return Mathf.RoundToInt(ScrollRect.horizontalNormalizedPosition * (ScrollRect.content.childCount - 1));
		}

		private void UpdatePaginator(int page)
		{
			if (Pagination)
			{
				_pageToggles[page].isOn = true;
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_drag = true;
			_dragTime = Time.time;
		}

		public void OnDrag(PointerEventData eventData)
		{
			var page = GetCurrentPage();

			if (page != _page)
			{
				_page = page;
				UpdatePaginator(page);
			}
		}
		
		public void OnEndDrag(PointerEventData eventData)
		{
			var delta = eventData.pressPosition.x - eventData.position.x;

			if (Mathf.Abs(delta) > SwipeThreshold && Time.time - _dragTime < SwipeTime)
			{
				var direction = Math.Sign(delta);

				Slide(direction);
			}

			_drag = false;
			_lerp = true;
		}
	}
}