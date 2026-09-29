export const PaymentMethod = {
  Cash: 1,
  BankTransfer: 2,
  Card: 3,
  MobileBanking: 4,
  Cheque: 5,
} as const;

export const PaymentStatus = {
  Completed: 1,
  Failed: 2,
  Refunded: 3,
  Voided: 4,
} as const;

export type PaymentMethodValue = (typeof PaymentMethod)[keyof typeof PaymentMethod];
export type PaymentStatusValue = (typeof PaymentStatus)[keyof typeof PaymentStatus];

export interface PaymentListDto {
  id: number;
  paymentNumber: string;
  studentName: string;
  amount: number;
  paymentDate: string;
  method: PaymentMethodValue;
  status: PaymentStatusValue;
  receiptNo?: string | null;
}

export interface PaymentDto extends PaymentListDto {
  invoiceId: number;
  invoiceNumber: string;
  studentId: number;
  transactionId?: string | null;
  remarks?: string | null;
  collectedByEmployeeId: number;
  collectedByEmployeeName: string;
  receiptId?: number | null;
  createdAt: string;
}

export interface CreatePaymentDto {
  invoiceId: number;
  studentId: number;
  amount: number;
  paymentDate: string;
  method: PaymentMethodValue;
  transactionId?: string | null;
  remarks?: string | null;
}

export interface VoidPaymentDto {
  reason: string;
}