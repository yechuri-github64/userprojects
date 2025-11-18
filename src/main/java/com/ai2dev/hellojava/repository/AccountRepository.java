package com.ai2dev.hellojava.repository; import com.ai2dev.hellojava.model.Account; import jakarta.inject.Singleton; import java.util.ArrayList; import java.util.List; @Singleton
public class AccountRepository { private List<Account> accounts = new ArrayList<> (); public List<Account> findAll () { return accounts; }

 public Account save (Account account) { accounts.add (account); return account; }

 public Account update (String id, Account account) { / / Update logic here
 return account; }

 public void delete (String id) { / / Delete logic here
 }
}