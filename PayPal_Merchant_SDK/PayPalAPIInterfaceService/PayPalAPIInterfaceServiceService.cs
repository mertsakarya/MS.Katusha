using PayPal;
using PayPal.Authentication;
using PayPal.Util;
using PayPal.Manager;
using PayPal.PayPalAPIInterfaceService.Model;

namespace PayPal.PayPalAPIInterfaceService {
	public partial class PayPalAPIInterfaceServiceService : BasePayPalService {

		// Service Version
		private static string ServiceVersion = "92.0";

		// Service Name
		private static string ServiceName = "PayPalAPIInterfaceService";

		public PayPalAPIInterfaceServiceService() : base(ServiceName, ServiceVersion)
		{
		}
	
		private void setStandardParams(AbstractRequestType request) {
			if (request.Version == null)
			{
				request.Version = ServiceVersion;
			}
			if (request.ErrorLanguage != null && ConfigManager.Instance.GetProperty("languageCode") != null)
			{
				request.ErrorLanguage = ConfigManager.Instance.GetProperty("languageCode");
			}
		}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public RefundTransactionResponseType RefundTransaction(RefundTransactionReq RefundTransactionReq, string apiUsername)
	 	{
			setStandardParams(RefundTransactionReq.RefundTransactionRequest);
		
			string resp = call("RefundTransaction", RefundTransactionReq.toXMLString(), apiUsername);
			return new RefundTransactionResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public RefundTransactionResponseType RefundTransaction(RefundTransactionReq RefundTransactionReq)
	 	{
	 		return RefundTransaction(RefundTransactionReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public InitiateRecoupResponseType InitiateRecoup(InitiateRecoupReq InitiateRecoupReq, string apiUsername)
	 	{
			setStandardParams(InitiateRecoupReq.InitiateRecoupRequest);
		
			string resp = call("InitiateRecoup", InitiateRecoupReq.toXMLString(), apiUsername);
			return new InitiateRecoupResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public InitiateRecoupResponseType InitiateRecoup(InitiateRecoupReq InitiateRecoupReq)
	 	{
	 		return InitiateRecoup(InitiateRecoupReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CompleteRecoupResponseType CompleteRecoup(CompleteRecoupReq CompleteRecoupReq, string apiUsername)
	 	{
			setStandardParams(CompleteRecoupReq.CompleteRecoupRequest);
		
			string resp = call("CompleteRecoup", CompleteRecoupReq.toXMLString(), apiUsername);
			return new CompleteRecoupResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CompleteRecoupResponseType CompleteRecoup(CompleteRecoupReq CompleteRecoupReq)
	 	{
	 		return CompleteRecoup(CompleteRecoupReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CancelRecoupResponseType CancelRecoup(CancelRecoupReq CancelRecoupReq, string apiUsername)
	 	{
			setStandardParams(CancelRecoupReq.CancelRecoupRequest);
		
			string resp = call("CancelRecoup", CancelRecoupReq.toXMLString(), apiUsername);
			return new CancelRecoupResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CancelRecoupResponseType CancelRecoup(CancelRecoupReq CancelRecoupReq)
	 	{
	 		return CancelRecoup(CancelRecoupReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetTransactionDetailsResponseType GetTransactionDetails(GetTransactionDetailsReq GetTransactionDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetTransactionDetailsReq.GetTransactionDetailsRequest);
		
			string resp = call("GetTransactionDetails", GetTransactionDetailsReq.toXMLString(), apiUsername);
			return new GetTransactionDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetTransactionDetailsResponseType GetTransactionDetails(GetTransactionDetailsReq GetTransactionDetailsReq)
	 	{
	 		return GetTransactionDetails(GetTransactionDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public BillUserResponseType BillUser(BillUserReq BillUserReq, string apiUsername)
	 	{
			setStandardParams(BillUserReq.BillUserRequest);
		
			string resp = call("BillUser", BillUserReq.toXMLString(), apiUsername);
			return new BillUserResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public BillUserResponseType BillUser(BillUserReq BillUserReq)
	 	{
	 		return BillUser(BillUserReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public TransactionSearchResponseType TransactionSearch(TransactionSearchReq TransactionSearchReq, string apiUsername)
	 	{
			setStandardParams(TransactionSearchReq.TransactionSearchRequest);
		
			string resp = call("TransactionSearch", TransactionSearchReq.toXMLString(), apiUsername);
			return new TransactionSearchResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public TransactionSearchResponseType TransactionSearch(TransactionSearchReq TransactionSearchReq)
	 	{
	 		return TransactionSearch(TransactionSearchReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public MassPayResponseType MassPay(MassPayReq MassPayReq, string apiUsername)
	 	{
			setStandardParams(MassPayReq.MassPayRequest);
		
			string resp = call("MassPay", MassPayReq.toXMLString(), apiUsername);
			return new MassPayResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public MassPayResponseType MassPay(MassPayReq MassPayReq)
	 	{
	 		return MassPay(MassPayReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public BAUpdateResponseType BillAgreementUpdate(BillAgreementUpdateReq BillAgreementUpdateReq, string apiUsername)
	 	{
			setStandardParams(BillAgreementUpdateReq.BAUpdateRequest);
		
			string resp = call("BillAgreementUpdate", BillAgreementUpdateReq.toXMLString(), apiUsername);
			return new BAUpdateResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public BAUpdateResponseType BillAgreementUpdate(BillAgreementUpdateReq BillAgreementUpdateReq)
	 	{
	 		return BillAgreementUpdate(BillAgreementUpdateReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public AddressVerifyResponseType AddressVerify(AddressVerifyReq AddressVerifyReq, string apiUsername)
	 	{
			setStandardParams(AddressVerifyReq.AddressVerifyRequest);
		
			string resp = call("AddressVerify", AddressVerifyReq.toXMLString(), apiUsername);
			return new AddressVerifyResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public AddressVerifyResponseType AddressVerify(AddressVerifyReq AddressVerifyReq)
	 	{
	 		return AddressVerify(AddressVerifyReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public EnterBoardingResponseType EnterBoarding(EnterBoardingReq EnterBoardingReq, string apiUsername)
	 	{
			setStandardParams(EnterBoardingReq.EnterBoardingRequest);
		
			string resp = call("EnterBoarding", EnterBoardingReq.toXMLString(), apiUsername);
			return new EnterBoardingResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public EnterBoardingResponseType EnterBoarding(EnterBoardingReq EnterBoardingReq)
	 	{
	 		return EnterBoarding(EnterBoardingReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetBoardingDetailsResponseType GetBoardingDetails(GetBoardingDetailsReq GetBoardingDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetBoardingDetailsReq.GetBoardingDetailsRequest);
		
			string resp = call("GetBoardingDetails", GetBoardingDetailsReq.toXMLString(), apiUsername);
			return new GetBoardingDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetBoardingDetailsResponseType GetBoardingDetails(GetBoardingDetailsReq GetBoardingDetailsReq)
	 	{
	 		return GetBoardingDetails(GetBoardingDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CreateMobilePaymentResponseType CreateMobilePayment(CreateMobilePaymentReq CreateMobilePaymentReq, string apiUsername)
	 	{
			setStandardParams(CreateMobilePaymentReq.CreateMobilePaymentRequest);
		
			string resp = call("CreateMobilePayment", CreateMobilePaymentReq.toXMLString(), apiUsername);
			return new CreateMobilePaymentResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CreateMobilePaymentResponseType CreateMobilePayment(CreateMobilePaymentReq CreateMobilePaymentReq)
	 	{
	 		return CreateMobilePayment(CreateMobilePaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetMobileStatusResponseType GetMobileStatus(GetMobileStatusReq GetMobileStatusReq, string apiUsername)
	 	{
			setStandardParams(GetMobileStatusReq.GetMobileStatusRequest);
		
			string resp = call("GetMobileStatus", GetMobileStatusReq.toXMLString(), apiUsername);
			return new GetMobileStatusResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetMobileStatusResponseType GetMobileStatus(GetMobileStatusReq GetMobileStatusReq)
	 	{
	 		return GetMobileStatus(GetMobileStatusReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetMobileCheckoutResponseType SetMobileCheckout(SetMobileCheckoutReq SetMobileCheckoutReq, string apiUsername)
	 	{
			setStandardParams(SetMobileCheckoutReq.SetMobileCheckoutRequest);
		
			string resp = call("SetMobileCheckout", SetMobileCheckoutReq.toXMLString(), apiUsername);
			return new SetMobileCheckoutResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetMobileCheckoutResponseType SetMobileCheckout(SetMobileCheckoutReq SetMobileCheckoutReq)
	 	{
	 		return SetMobileCheckout(SetMobileCheckoutReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoMobileCheckoutPaymentResponseType DoMobileCheckoutPayment(DoMobileCheckoutPaymentReq DoMobileCheckoutPaymentReq, string apiUsername)
	 	{
			setStandardParams(DoMobileCheckoutPaymentReq.DoMobileCheckoutPaymentRequest);
		
			string resp = call("DoMobileCheckoutPayment", DoMobileCheckoutPaymentReq.toXMLString(), apiUsername);
			return new DoMobileCheckoutPaymentResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoMobileCheckoutPaymentResponseType DoMobileCheckoutPayment(DoMobileCheckoutPaymentReq DoMobileCheckoutPaymentReq)
	 	{
	 		return DoMobileCheckoutPayment(DoMobileCheckoutPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetBalanceResponseType GetBalance(GetBalanceReq GetBalanceReq, string apiUsername)
	 	{
			setStandardParams(GetBalanceReq.GetBalanceRequest);
		
			string resp = call("GetBalance", GetBalanceReq.toXMLString(), apiUsername);
			return new GetBalanceResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetBalanceResponseType GetBalance(GetBalanceReq GetBalanceReq)
	 	{
	 		return GetBalance(GetBalanceReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetPalDetailsResponseType GetPalDetails(GetPalDetailsReq GetPalDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetPalDetailsReq.GetPalDetailsRequest);
		
			string resp = call("GetPalDetails", GetPalDetailsReq.toXMLString(), apiUsername);
			return new GetPalDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetPalDetailsResponseType GetPalDetails(GetPalDetailsReq GetPalDetailsReq)
	 	{
	 		return GetPalDetails(GetPalDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoExpressCheckoutPaymentResponseType DoExpressCheckoutPayment(DoExpressCheckoutPaymentReq DoExpressCheckoutPaymentReq, string apiUsername)
	 	{
			setStandardParams(DoExpressCheckoutPaymentReq.DoExpressCheckoutPaymentRequest);
		
			string resp = call("DoExpressCheckoutPayment", DoExpressCheckoutPaymentReq.toXMLString(), apiUsername);
			return new DoExpressCheckoutPaymentResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoExpressCheckoutPaymentResponseType DoExpressCheckoutPayment(DoExpressCheckoutPaymentReq DoExpressCheckoutPaymentReq)
	 	{
	 		return DoExpressCheckoutPayment(DoExpressCheckoutPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoUATPExpressCheckoutPaymentResponseType DoUATPExpressCheckoutPayment(DoUATPExpressCheckoutPaymentReq DoUATPExpressCheckoutPaymentReq, string apiUsername)
	 	{
			setStandardParams(DoUATPExpressCheckoutPaymentReq.DoUATPExpressCheckoutPaymentRequest);
		
			string resp = call("DoUATPExpressCheckoutPayment", DoUATPExpressCheckoutPaymentReq.toXMLString(), apiUsername);
			return new DoUATPExpressCheckoutPaymentResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoUATPExpressCheckoutPaymentResponseType DoUATPExpressCheckoutPayment(DoUATPExpressCheckoutPaymentReq DoUATPExpressCheckoutPaymentReq)
	 	{
	 		return DoUATPExpressCheckoutPayment(DoUATPExpressCheckoutPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetAuthFlowParamResponseType SetAuthFlowParam(SetAuthFlowParamReq SetAuthFlowParamReq, string apiUsername)
	 	{
			setStandardParams(SetAuthFlowParamReq.SetAuthFlowParamRequest);
		
			string resp = call("SetAuthFlowParam", SetAuthFlowParamReq.toXMLString(), apiUsername);
			return new SetAuthFlowParamResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetAuthFlowParamResponseType SetAuthFlowParam(SetAuthFlowParamReq SetAuthFlowParamReq)
	 	{
	 		return SetAuthFlowParam(SetAuthFlowParamReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetAuthDetailsResponseType GetAuthDetails(GetAuthDetailsReq GetAuthDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetAuthDetailsReq.GetAuthDetailsRequest);
		
			string resp = call("GetAuthDetails", GetAuthDetailsReq.toXMLString(), apiUsername);
			return new GetAuthDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetAuthDetailsResponseType GetAuthDetails(GetAuthDetailsReq GetAuthDetailsReq)
	 	{
	 		return GetAuthDetails(GetAuthDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetAccessPermissionsResponseType SetAccessPermissions(SetAccessPermissionsReq SetAccessPermissionsReq, string apiUsername)
	 	{
			setStandardParams(SetAccessPermissionsReq.SetAccessPermissionsRequest);
		
			string resp = call("SetAccessPermissions", SetAccessPermissionsReq.toXMLString(), apiUsername);
			return new SetAccessPermissionsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetAccessPermissionsResponseType SetAccessPermissions(SetAccessPermissionsReq SetAccessPermissionsReq)
	 	{
	 		return SetAccessPermissions(SetAccessPermissionsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public UpdateAccessPermissionsResponseType UpdateAccessPermissions(UpdateAccessPermissionsReq UpdateAccessPermissionsReq, string apiUsername)
	 	{
			setStandardParams(UpdateAccessPermissionsReq.UpdateAccessPermissionsRequest);
		
			string resp = call("UpdateAccessPermissions", UpdateAccessPermissionsReq.toXMLString(), apiUsername);
			return new UpdateAccessPermissionsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public UpdateAccessPermissionsResponseType UpdateAccessPermissions(UpdateAccessPermissionsReq UpdateAccessPermissionsReq)
	 	{
	 		return UpdateAccessPermissions(UpdateAccessPermissionsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetAccessPermissionDetailsResponseType GetAccessPermissionDetails(GetAccessPermissionDetailsReq GetAccessPermissionDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetAccessPermissionDetailsReq.GetAccessPermissionDetailsRequest);
		
			string resp = call("GetAccessPermissionDetails", GetAccessPermissionDetailsReq.toXMLString(), apiUsername);
			return new GetAccessPermissionDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetAccessPermissionDetailsResponseType GetAccessPermissionDetails(GetAccessPermissionDetailsReq GetAccessPermissionDetailsReq)
	 	{
	 		return GetAccessPermissionDetails(GetAccessPermissionDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetIncentiveEvaluationResponseType GetIncentiveEvaluation(GetIncentiveEvaluationReq GetIncentiveEvaluationReq, string apiUsername)
	 	{
			setStandardParams(GetIncentiveEvaluationReq.GetIncentiveEvaluationRequest);
		
			string resp = call("GetIncentiveEvaluation", GetIncentiveEvaluationReq.toXMLString(), apiUsername);
			return new GetIncentiveEvaluationResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetIncentiveEvaluationResponseType GetIncentiveEvaluation(GetIncentiveEvaluationReq GetIncentiveEvaluationReq)
	 	{
	 		return GetIncentiveEvaluation(GetIncentiveEvaluationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutReq SetExpressCheckoutReq, string apiUsername)
	 	{
			setStandardParams(SetExpressCheckoutReq.SetExpressCheckoutRequest);
		
			string resp = call("SetExpressCheckout", SetExpressCheckoutReq.toXMLString(), apiUsername);
			return new SetExpressCheckoutResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutReq SetExpressCheckoutReq)
	 	{
	 		return SetExpressCheckout(SetExpressCheckoutReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ExecuteCheckoutOperationsResponseType ExecuteCheckoutOperations(ExecuteCheckoutOperationsReq ExecuteCheckoutOperationsReq, string apiUsername)
	 	{
			setStandardParams(ExecuteCheckoutOperationsReq.ExecuteCheckoutOperationsRequest);
		
			string resp = call("ExecuteCheckoutOperations", ExecuteCheckoutOperationsReq.toXMLString(), apiUsername);
			return new ExecuteCheckoutOperationsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ExecuteCheckoutOperationsResponseType ExecuteCheckoutOperations(ExecuteCheckoutOperationsReq ExecuteCheckoutOperationsReq)
	 	{
	 		return ExecuteCheckoutOperations(ExecuteCheckoutOperationsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(GetExpressCheckoutDetailsReq GetExpressCheckoutDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetExpressCheckoutDetailsReq.GetExpressCheckoutDetailsRequest);
		
			string resp = call("GetExpressCheckoutDetails", GetExpressCheckoutDetailsReq.toXMLString(), apiUsername);
			return new GetExpressCheckoutDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(GetExpressCheckoutDetailsReq GetExpressCheckoutDetailsReq)
	 	{
	 		return GetExpressCheckoutDetails(GetExpressCheckoutDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoDirectPaymentResponseType DoDirectPayment(DoDirectPaymentReq DoDirectPaymentReq, string apiUsername)
	 	{
			setStandardParams(DoDirectPaymentReq.DoDirectPaymentRequest);
		
			string resp = call("DoDirectPayment", DoDirectPaymentReq.toXMLString(), apiUsername);
			return new DoDirectPaymentResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoDirectPaymentResponseType DoDirectPayment(DoDirectPaymentReq DoDirectPaymentReq)
	 	{
	 		return DoDirectPayment(DoDirectPaymentReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ManagePendingTransactionStatusResponseType ManagePendingTransactionStatus(ManagePendingTransactionStatusReq ManagePendingTransactionStatusReq, string apiUsername)
	 	{
			setStandardParams(ManagePendingTransactionStatusReq.ManagePendingTransactionStatusRequest);
		
			string resp = call("ManagePendingTransactionStatus", ManagePendingTransactionStatusReq.toXMLString(), apiUsername);
			return new ManagePendingTransactionStatusResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ManagePendingTransactionStatusResponseType ManagePendingTransactionStatus(ManagePendingTransactionStatusReq ManagePendingTransactionStatusReq)
	 	{
	 		return ManagePendingTransactionStatus(ManagePendingTransactionStatusReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoCancelResponseType DoCancel(DoCancelReq DoCancelReq, string apiUsername)
	 	{
			setStandardParams(DoCancelReq.DoCancelRequest);
		
			string resp = call("DoCancel", DoCancelReq.toXMLString(), apiUsername);
			return new DoCancelResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoCancelResponseType DoCancel(DoCancelReq DoCancelReq)
	 	{
	 		return DoCancel(DoCancelReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoCaptureResponseType DoCapture(DoCaptureReq DoCaptureReq, string apiUsername)
	 	{
			setStandardParams(DoCaptureReq.DoCaptureRequest);
		
			string resp = call("DoCapture", DoCaptureReq.toXMLString(), apiUsername);
			return new DoCaptureResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoCaptureResponseType DoCapture(DoCaptureReq DoCaptureReq)
	 	{
	 		return DoCapture(DoCaptureReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoReauthorizationResponseType DoReauthorization(DoReauthorizationReq DoReauthorizationReq, string apiUsername)
	 	{
			setStandardParams(DoReauthorizationReq.DoReauthorizationRequest);
		
			string resp = call("DoReauthorization", DoReauthorizationReq.toXMLString(), apiUsername);
			return new DoReauthorizationResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoReauthorizationResponseType DoReauthorization(DoReauthorizationReq DoReauthorizationReq)
	 	{
	 		return DoReauthorization(DoReauthorizationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoVoidResponseType DoVoid(DoVoidReq DoVoidReq, string apiUsername)
	 	{
			setStandardParams(DoVoidReq.DoVoidRequest);
		
			string resp = call("DoVoid", DoVoidReq.toXMLString(), apiUsername);
			return new DoVoidResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoVoidResponseType DoVoid(DoVoidReq DoVoidReq)
	 	{
	 		return DoVoid(DoVoidReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoAuthorizationResponseType DoAuthorization(DoAuthorizationReq DoAuthorizationReq, string apiUsername)
	 	{
			setStandardParams(DoAuthorizationReq.DoAuthorizationRequest);
		
			string resp = call("DoAuthorization", DoAuthorizationReq.toXMLString(), apiUsername);
			return new DoAuthorizationResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoAuthorizationResponseType DoAuthorization(DoAuthorizationReq DoAuthorizationReq)
	 	{
	 		return DoAuthorization(DoAuthorizationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public SetCustomerBillingAgreementResponseType SetCustomerBillingAgreement(SetCustomerBillingAgreementReq SetCustomerBillingAgreementReq, string apiUsername)
	 	{
			setStandardParams(SetCustomerBillingAgreementReq.SetCustomerBillingAgreementRequest);
		
			string resp = call("SetCustomerBillingAgreement", SetCustomerBillingAgreementReq.toXMLString(), apiUsername);
			return new SetCustomerBillingAgreementResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public SetCustomerBillingAgreementResponseType SetCustomerBillingAgreement(SetCustomerBillingAgreementReq SetCustomerBillingAgreementReq)
	 	{
	 		return SetCustomerBillingAgreement(SetCustomerBillingAgreementReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetBillingAgreementCustomerDetailsResponseType GetBillingAgreementCustomerDetails(GetBillingAgreementCustomerDetailsReq GetBillingAgreementCustomerDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetBillingAgreementCustomerDetailsReq.GetBillingAgreementCustomerDetailsRequest);
		
			string resp = call("GetBillingAgreementCustomerDetails", GetBillingAgreementCustomerDetailsReq.toXMLString(), apiUsername);
			return new GetBillingAgreementCustomerDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetBillingAgreementCustomerDetailsResponseType GetBillingAgreementCustomerDetails(GetBillingAgreementCustomerDetailsReq GetBillingAgreementCustomerDetailsReq)
	 	{
	 		return GetBillingAgreementCustomerDetails(GetBillingAgreementCustomerDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CreateBillingAgreementResponseType CreateBillingAgreement(CreateBillingAgreementReq CreateBillingAgreementReq, string apiUsername)
	 	{
			setStandardParams(CreateBillingAgreementReq.CreateBillingAgreementRequest);
		
			string resp = call("CreateBillingAgreement", CreateBillingAgreementReq.toXMLString(), apiUsername);
			return new CreateBillingAgreementResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CreateBillingAgreementResponseType CreateBillingAgreement(CreateBillingAgreementReq CreateBillingAgreementReq)
	 	{
	 		return CreateBillingAgreement(CreateBillingAgreementReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoReferenceTransactionResponseType DoReferenceTransaction(DoReferenceTransactionReq DoReferenceTransactionReq, string apiUsername)
	 	{
			setStandardParams(DoReferenceTransactionReq.DoReferenceTransactionRequest);
		
			string resp = call("DoReferenceTransaction", DoReferenceTransactionReq.toXMLString(), apiUsername);
			return new DoReferenceTransactionResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoReferenceTransactionResponseType DoReferenceTransaction(DoReferenceTransactionReq DoReferenceTransactionReq)
	 	{
	 		return DoReferenceTransaction(DoReferenceTransactionReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoNonReferencedCreditResponseType DoNonReferencedCredit(DoNonReferencedCreditReq DoNonReferencedCreditReq, string apiUsername)
	 	{
			setStandardParams(DoNonReferencedCreditReq.DoNonReferencedCreditRequest);
		
			string resp = call("DoNonReferencedCredit", DoNonReferencedCreditReq.toXMLString(), apiUsername);
			return new DoNonReferencedCreditResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoNonReferencedCreditResponseType DoNonReferencedCredit(DoNonReferencedCreditReq DoNonReferencedCreditReq)
	 	{
	 		return DoNonReferencedCredit(DoNonReferencedCreditReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public DoUATPAuthorizationResponseType DoUATPAuthorization(DoUATPAuthorizationReq DoUATPAuthorizationReq, string apiUsername)
	 	{
			setStandardParams(DoUATPAuthorizationReq.DoUATPAuthorizationRequest);
		
			string resp = call("DoUATPAuthorization", DoUATPAuthorizationReq.toXMLString(), apiUsername);
			return new DoUATPAuthorizationResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public DoUATPAuthorizationResponseType DoUATPAuthorization(DoUATPAuthorizationReq DoUATPAuthorizationReq)
	 	{
	 		return DoUATPAuthorization(DoUATPAuthorizationReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public CreateRecurringPaymentsProfileResponseType CreateRecurringPaymentsProfile(CreateRecurringPaymentsProfileReq CreateRecurringPaymentsProfileReq, string apiUsername)
	 	{
			setStandardParams(CreateRecurringPaymentsProfileReq.CreateRecurringPaymentsProfileRequest);
		
			string resp = call("CreateRecurringPaymentsProfile", CreateRecurringPaymentsProfileReq.toXMLString(), apiUsername);
			return new CreateRecurringPaymentsProfileResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public CreateRecurringPaymentsProfileResponseType CreateRecurringPaymentsProfile(CreateRecurringPaymentsProfileReq CreateRecurringPaymentsProfileReq)
	 	{
	 		return CreateRecurringPaymentsProfile(CreateRecurringPaymentsProfileReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public GetRecurringPaymentsProfileDetailsResponseType GetRecurringPaymentsProfileDetails(GetRecurringPaymentsProfileDetailsReq GetRecurringPaymentsProfileDetailsReq, string apiUsername)
	 	{
			setStandardParams(GetRecurringPaymentsProfileDetailsReq.GetRecurringPaymentsProfileDetailsRequest);
		
			string resp = call("GetRecurringPaymentsProfileDetails", GetRecurringPaymentsProfileDetailsReq.toXMLString(), apiUsername);
			return new GetRecurringPaymentsProfileDetailsResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public GetRecurringPaymentsProfileDetailsResponseType GetRecurringPaymentsProfileDetails(GetRecurringPaymentsProfileDetailsReq GetRecurringPaymentsProfileDetailsReq)
	 	{
	 		return GetRecurringPaymentsProfileDetails(GetRecurringPaymentsProfileDetailsReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ManageRecurringPaymentsProfileStatusResponseType ManageRecurringPaymentsProfileStatus(ManageRecurringPaymentsProfileStatusReq ManageRecurringPaymentsProfileStatusReq, string apiUsername)
	 	{
			setStandardParams(ManageRecurringPaymentsProfileStatusReq.ManageRecurringPaymentsProfileStatusRequest);
		
			string resp = call("ManageRecurringPaymentsProfileStatus", ManageRecurringPaymentsProfileStatusReq.toXMLString(), apiUsername);
			return new ManageRecurringPaymentsProfileStatusResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ManageRecurringPaymentsProfileStatusResponseType ManageRecurringPaymentsProfileStatus(ManageRecurringPaymentsProfileStatusReq ManageRecurringPaymentsProfileStatusReq)
	 	{
	 		return ManageRecurringPaymentsProfileStatus(ManageRecurringPaymentsProfileStatusReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public BillOutstandingAmountResponseType BillOutstandingAmount(BillOutstandingAmountReq BillOutstandingAmountReq, string apiUsername)
	 	{
			setStandardParams(BillOutstandingAmountReq.BillOutstandingAmountRequest);
		
			string resp = call("BillOutstandingAmount", BillOutstandingAmountReq.toXMLString(), apiUsername);
			return new BillOutstandingAmountResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public BillOutstandingAmountResponseType BillOutstandingAmount(BillOutstandingAmountReq BillOutstandingAmountReq)
	 	{
	 		return BillOutstandingAmount(BillOutstandingAmountReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public UpdateRecurringPaymentsProfileResponseType UpdateRecurringPaymentsProfile(UpdateRecurringPaymentsProfileReq UpdateRecurringPaymentsProfileReq, string apiUsername)
	 	{
			setStandardParams(UpdateRecurringPaymentsProfileReq.UpdateRecurringPaymentsProfileRequest);
		
			string resp = call("UpdateRecurringPaymentsProfile", UpdateRecurringPaymentsProfileReq.toXMLString(), apiUsername);
			return new UpdateRecurringPaymentsProfileResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public UpdateRecurringPaymentsProfileResponseType UpdateRecurringPaymentsProfile(UpdateRecurringPaymentsProfileReq UpdateRecurringPaymentsProfileReq)
	 	{
	 		return UpdateRecurringPaymentsProfile(UpdateRecurringPaymentsProfileReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ReverseTransactionResponseType ReverseTransaction(ReverseTransactionReq ReverseTransactionReq, string apiUsername)
	 	{
			setStandardParams(ReverseTransactionReq.ReverseTransactionRequest);
		
			string resp = call("ReverseTransaction", ReverseTransactionReq.toXMLString(), apiUsername);
			return new ReverseTransactionResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ReverseTransactionResponseType ReverseTransaction(ReverseTransactionReq ReverseTransactionReq)
	 	{
	 		return ReverseTransaction(ReverseTransactionReq, null);
	 	}

		/**	
          *AUTO_GENERATED
	 	  */
	 	public ExternalRememberMeOptOutResponseType ExternalRememberMeOptOut(ExternalRememberMeOptOutReq ExternalRememberMeOptOutReq, string apiUsername)
	 	{
			setStandardParams(ExternalRememberMeOptOutReq.ExternalRememberMeOptOutRequest);
		
			string resp = call("ExternalRememberMeOptOut", ExternalRememberMeOptOutReq.toXMLString(), apiUsername);
			return new ExternalRememberMeOptOutResponseType(resp);
	 	}
	 
	 	/** 
          *AUTO_GENERATED
	 	  */
	 	public ExternalRememberMeOptOutResponseType ExternalRememberMeOptOut(ExternalRememberMeOptOutReq ExternalRememberMeOptOutReq)
	 	{
	 		return ExternalRememberMeOptOut(ExternalRememberMeOptOutReq, null);
	 	}
	}
}
