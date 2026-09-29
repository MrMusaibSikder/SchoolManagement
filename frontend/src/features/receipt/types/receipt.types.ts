export interface ReceiptDto {
  id: number;
  receiptNo: string;
  paymentId: number;
  paymentNumber: string;
  issuedAt: string;
  issuedByEmployeeId: number;
  issuedByEmployeeName: string;
  isVoided: boolean;
  voidedAt?: string | null;
  voidReason?: string | null;
  createdAt: string;
}

export interface VoidReceiptDto {
  voidReason: string;
}