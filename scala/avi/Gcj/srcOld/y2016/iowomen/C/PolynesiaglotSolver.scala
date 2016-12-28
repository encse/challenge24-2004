package y2016.iowomen.C

import cmn._

object PolynesiaglotSolver extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out) = {
    val List(cCons, cVowel, length) = fetch(PList(PBigInt))

    val modulo = BigInt(1000000007)

    var f: BigInt => BigInt = null
    f = cache({
      case x if x == 0 => 0
      case x if x == 1 => cVowel
      case x if x == 2 => (cVowel*cVowel + cCons * cVowel) % modulo
      case x => (cVowel * f(x - 1) + cCons * cVowel * f(x - 2)) % modulo
    })

    out(f(length))
  }
}
