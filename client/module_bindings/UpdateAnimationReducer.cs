// THIS FILE IS AUTOMATICALLY GENERATED BY SPACETIMEDB. EDITS TO THIS FILE
// WILL NOT BE SAVED. MODIFY TABLES IN RUST INSTEAD.

using System;
using ClientApi;
using Newtonsoft.Json.Linq;
using SpacetimeDB;

namespace SpacetimeDB.Types
{
	public static partial class Reducer
	{
		public delegate void UpdateAnimationHandler(ReducerEvent reducerEvent, ulong entityId, bool moving, ulong actionTargetEntityId);
		public static event UpdateAnimationHandler OnUpdateAnimationEvent;

		public static void UpdateAnimation(ulong entityId, bool moving, ulong actionTargetEntityId)
		{
			var _argArray = new object[] {entityId, moving, actionTargetEntityId};
			var _message = new SpacetimeDBClient.ReducerCallRequest {
				fn = "update_animation",
				args = _argArray,
			};
			Newtonsoft.Json.JsonSerializerSettings _settings = new Newtonsoft.Json.JsonSerializerSettings
			{
				Converters = { new SpacetimeDB.SomeWrapperConverter(), new SpacetimeDB.EnumWrapperConverter() },
				ContractResolver = new SpacetimeDB.JsonContractResolver(),
			};
			SpacetimeDBClient.instance.InternalCallReducer(Newtonsoft.Json.JsonConvert.SerializeObject(_message, _settings));
		}

		[ReducerCallback(FunctionName = "update_animation")]
		public static bool OnUpdateAnimation(ClientApi.Event dbEvent)
		{
			if(OnUpdateAnimationEvent != null)
			{
				var args = ((ReducerEvent)dbEvent.FunctionCall.CallInfo).UpdateAnimationArgs;
				OnUpdateAnimationEvent((ReducerEvent)dbEvent.FunctionCall.CallInfo
					,(ulong)args.EntityId
					,(bool)args.Moving
					,(ulong)args.ActionTargetEntityId
				);
				return true;
			}
			return false;
		}

		[DeserializeEvent(FunctionName = "update_animation")]
		public static void UpdateAnimationDeserializeEventArgs(ClientApi.Event dbEvent)
		{
			var args = new UpdateAnimationArgsStruct();
			var bsatnBytes = dbEvent.FunctionCall.ArgBytes;
			using var ms = new System.IO.MemoryStream();
			ms.SetLength(bsatnBytes.Length);
			bsatnBytes.CopyTo(ms.GetBuffer(), 0);
			ms.Position = 0;
			using var reader = new System.IO.BinaryReader(ms);
			var args_0_value = SpacetimeDB.SATS.AlgebraicValue.Deserialize(SpacetimeDB.SATS.AlgebraicType.CreatePrimitiveType(SpacetimeDB.SATS.BuiltinType.Type.U64), reader);
			args.EntityId = args_0_value.AsU64();
			var args_1_value = SpacetimeDB.SATS.AlgebraicValue.Deserialize(SpacetimeDB.SATS.AlgebraicType.CreatePrimitiveType(SpacetimeDB.SATS.BuiltinType.Type.Bool), reader);
			args.Moving = args_1_value.AsBool();
			var args_2_value = SpacetimeDB.SATS.AlgebraicValue.Deserialize(SpacetimeDB.SATS.AlgebraicType.CreatePrimitiveType(SpacetimeDB.SATS.BuiltinType.Type.U64), reader);
			args.ActionTargetEntityId = args_2_value.AsU64();
			dbEvent.FunctionCall.CallInfo = new ReducerEvent(ReducerType.UpdateAnimation, "update_animation", dbEvent.Timestamp, Identity.From(dbEvent.CallerIdentity.ToByteArray()), dbEvent.Message, dbEvent.Status, args);
		}
	}

	public partial class UpdateAnimationArgsStruct
	{
		public ulong EntityId;
		public bool Moving;
		public ulong ActionTargetEntityId;
	}

}
