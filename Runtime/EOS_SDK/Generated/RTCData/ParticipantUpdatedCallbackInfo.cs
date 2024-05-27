// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCData
{
	/// <summary>
	/// This struct is passed in with a call to <see cref="OnParticipantUpdatedCallback" /> registered event.
	/// </summary>
	public struct ParticipantUpdatedCallbackInfo : ICallbackInfo
	{
		/// <summary>
		/// Client-specified data passed into <see cref="RTCDataInterface.AddNotifyParticipantUpdated" />.
		/// </summary>
		public object ClientData { get; set; }

		/// <summary>
		/// The Product User ID of the user who initiated this request.
		/// </summary>
		public ProductUserId LocalUserId { get; set; }

		/// <summary>
		/// The room associated with this event.
		/// </summary>
		public Utf8String RoomName { get; set; }

		/// <summary>
		/// The participant updated.
		/// </summary>
		public ProductUserId ParticipantId { get; set; }

		/// <summary>
		/// The data channel status.
		/// </summary>
		public RTCDataStatus DataStatus { get; set; }

		public Result? GetResultCode()
		{
			return null;
		}

		internal void Set(ref ParticipantUpdatedCallbackInfoInternal other)
		{
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			ParticipantId = other.ParticipantId;
			DataStatus = other.DataStatus;
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct ParticipantUpdatedCallbackInfoInternal : ICallbackInfoInternal, IGettable<ParticipantUpdatedCallbackInfo>, ISettable<ParticipantUpdatedCallbackInfo>, System.IDisposable
	{
		private System.IntPtr m_ClientData;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_RoomName;
		private System.IntPtr m_ParticipantId;
		private RTCDataStatus m_DataStatus;

		public object ClientData
		{
			get
			{
				object value;
				Helper.Get(m_ClientData, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_ClientData);
			}
		}

		public System.IntPtr ClientDataAddress
		{
			get
			{
				return m_ClientData;
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId value;
				Helper.Get(m_LocalUserId, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_LocalUserId);
			}
		}

		public Utf8String RoomName
		{
			get
			{
				Utf8String value;
				Helper.Get(m_RoomName, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_RoomName);
			}
		}

		public ProductUserId ParticipantId
		{
			get
			{
				ProductUserId value;
				Helper.Get(m_ParticipantId, out value);
				return value;
			}

			set
			{
				Helper.Set(value, ref m_ParticipantId);
			}
		}

		public RTCDataStatus DataStatus
		{
			get
			{
				return m_DataStatus;
			}

			set
			{
				m_DataStatus = value;
			}
		}

		public void Set(ref ParticipantUpdatedCallbackInfo other)
		{
			ClientData = other.ClientData;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			ParticipantId = other.ParticipantId;
			DataStatus = other.DataStatus;
		}

		public void Set(ref ParticipantUpdatedCallbackInfo? other)
		{
			if (other.HasValue)
			{
				ClientData = other.Value.ClientData;
				LocalUserId = other.Value.LocalUserId;
				RoomName = other.Value.RoomName;
				ParticipantId = other.Value.ParticipantId;
				DataStatus = other.Value.DataStatus;
			}
		}

		public void Dispose()
		{
			Helper.Dispose(ref m_ClientData);
			Helper.Dispose(ref m_LocalUserId);
			Helper.Dispose(ref m_RoomName);
			Helper.Dispose(ref m_ParticipantId);
		}

		public void Get(out ParticipantUpdatedCallbackInfo output)
		{
			output = new ParticipantUpdatedCallbackInfo();
			output.Set(ref this);
		}
	}
}