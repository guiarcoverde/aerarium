import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateTransactionRequest } from '../../models/transaction';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly http = inject(HttpClient);

  create(request: CreateTransactionRequest): Observable<void> {
    return this.http.post<void>(
      `${environment.apiUrl}/transactions`,
      request,
    );
  }
}
